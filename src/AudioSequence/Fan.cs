using System.Collections;
using UnityEngine;

namespace AudioMod.AudioSequence
{
    public class Fan : AudioSequence<Fan>
    {
        // === Inspector Tunables ================================================
        [Header("Curves (X = normalized RPM 0..1)")]
        private readonly AnimationCurve _volumeCurve = AnimationCurve.EaseInOut(0f, 0.04f, 1f, 0.1f);
        private readonly AnimationCurve _pitchCurve = AnimationCurve.EaseInOut(0f, 0.7f, 1f, 1.2f);

        [Header("RPM Model")] 
        [Range(200f, 3000f)] private const float MinRpm = 400f;
        [Range(1000f, 8000f)] private const float MaxRpm = 3200f;
        private const float AccelRpmPerSec = 900f;
        private const float DecelRpmPerSec = 600f;

        [Header("Thermal Model")] 
        [Range(0f, 1f)] private const float AmbientLoad = 0.03f;
        private const float HeatRiseTime = 6f;
        private const float CoolFallTime = 12f;

        [Header("Load Inputs (0..1)")]
        [SerializeField, Range(0f, 1f)] private float _memoryUsage;

        [Range(0f, 1f)] private const float MemoryWeight = 0.35f;
        private const bool IncludeFrameLoad = true;
        [Range(20, 240)] private const int TargetFps = 60;
        [Range(0f, 1f)] private const float FrameLoadWeight = 0.45f;
        [Range(0f, 1f)] private const float NoiseWeight = 0.20f;

        [Header("Micro Modulation (Character)")] 
        [Range(0.05f, 2f)] private const float WobbleHz = 0.33f;
        [Range(0f, 0.2f)] private const float WobbleVolDepth = 0.06f;
        [Range(0f, 0.05f)] private const float WobblePitchDepth = 0.0125f;
        [Range(0.5f, 12f)] private const float WowHz = 2.5f;
        [Range(0f, 0.04f)] private const float WowPitchDepth = 0.01f;
        [Range(0f, 0.12f)] private const float WowVolDepth = 0.05f;

        [Header("Startup/Shutdown")] 
        private const float StartupAccelMultiplier = 1.4f;
        private const float ShutdownDecelMultiplier = 4f;

        // === Internal State =====================================================
        private float _rpm = 0f;
        private float _targetRpm = 0f;
        private float _thermal = 0f;
        private float _perlinSeed = 0f;

        protected override AudioClip[] _clips => new[]
        {
            AudioLib.CPUFanLoop
        };

        public void setMemoryUsage(float usage) => _memoryUsage = usage;

        public override void Start()
        {
            Init();

            _perlinSeed = Random.value * 1000f;
            _thermal = AmbientLoad;

            Sources[0].loop = true;
            Sources[0].Play();

            // Start from a true stop (0 RPM), muted, and let the startup ramp take over.
            _rpm = 0f;
            var normAtZero = 0f; // normalized 0 for pitch/vol curves
            ApplyAudio(normAtZero, 1f, 0f); // volume 0 at boot

            base.Start();
        }

        protected override IEnumerator Loop()
        {
            // ========= STARTUP RAMP (spin-up from 0 to initial target) ==========
            // Compute an initial target from current thermal/load so the ramp aims correctly.
            {
                var dt = 0f;
                // Seed load/thermal once so initial target feels plausible.
                UpdateThermal(ComputeLoad(Time.deltaTime), Time.deltaTime);

                var norm = Mathf.Clamp01(Mathf.Max(AmbientLoad, _thermal));
                _targetRpm = Mathf.Lerp(MinRpm, MaxRpm, norm);

                // Ramp from 0 to at least a quiet idle (quarter of min), then toward target.
                var idleRpm = Mathf.Max(MinRpm * 0.25f, 0f);
                while (_rpm < Mathf.Min(idleRpm, _targetRpm))
                {
                    dt = Time.deltaTime;
                    _rpm = Mathf.MoveTowards(_rpm, idleRpm, AccelRpmPerSec * StartupAccelMultiplier * dt);

                    var rpmNorm = Mathf.Clamp01(Mathf.InverseLerp(MinRpm, MaxRpm, Mathf.Max(_rpm, MinRpm)));
                    // Fade in volume from 0 to whatever the curve says at current rpm.
                    var volEnvelope = Mathf.InverseLerp(0f, idleRpm, _rpm);
                    ApplyAudio(rpmNorm, 1f, volEnvelope);
                    yield return null;
                }

                // Continue ramping toward computed target smoothly
                while (_rpm < _targetRpm && !isStopping)
                {
                    dt = Time.deltaTime;

                    // Keep updating thermal to avoid a static target during ramp
                    var load = ComputeLoad(dt);
                    UpdateThermal(load, dt);
                    var normNow = Mathf.Clamp01(Mathf.Max(AmbientLoad, _thermal));
                    _targetRpm = Mathf.Lerp(MinRpm, MaxRpm, normNow);

                    var limit = AccelRpmPerSec * StartupAccelMultiplier * dt;
                    _rpm = Mathf.MoveTowards(_rpm, _targetRpm, limit);

                    var rpmNorm = Mathf.Clamp01(Mathf.InverseLerp(MinRpm, MaxRpm, _rpm));
                    ApplyAudio(rpmNorm, 1f, 1f);
                    yield return null;
                }
            }

            // ======================== MAIN RUN LOOP =============================
            while (!isStopping)
            {
                yield return null;

                var dt = Time.deltaTime;
                var load = ComputeLoad(dt);
                UpdateThermal(load, dt);

                var norm = Mathf.Clamp01(Mathf.Max(AmbientLoad, _thermal));
                _targetRpm = Mathf.Lerp(MinRpm, MaxRpm, norm);

                var limit = (_targetRpm > _rpm ? AccelRpmPerSec : DecelRpmPerSec) * dt;
                _rpm = Mathf.MoveTowards(_rpm, _targetRpm, limit);

                var rpmNorm = Mathf.InverseLerp(MinRpm, MaxRpm, _rpm);

                var t = Time.time;
                var wobble = Mathf.Sin(t * Mathf.PI * 2f * WobbleHz);
                var perlin = Mathf.PerlinNoise(_perlinSeed, t * WowHz) * 2f - 1f;

                var pitchMod = 1f + (wobble * WobblePitchDepth) + (perlin * WowPitchDepth);
                var volMod = 1f + (wobble * WobbleVolDepth) + (perlin * WowVolDepth);

                ApplyAudio(rpmNorm, pitchMod, volMod);
            }

            // ======================= SHUTDOWN RAMP ==============================
            // Smoothly spin down to a full stop (RPM→0, volume fade to 0).
            while (_rpm > 0f || Sources[0].volume > 0.001f)
            {
                var dt = Time.deltaTime;

                // Decelerate faster using the shutdown multiplier.
                _rpm = Mathf.MoveTowards(_rpm, 0f, DecelRpmPerSec * ShutdownDecelMultiplier * dt);

                // Use curve-based pitch, but fade volume with an envelope to silence.
                var rpmNorm = Mathf.Clamp01(Mathf.InverseLerp(MinRpm, MaxRpm, Mathf.Max(_rpm, 0f)));
                var volEnvelope = Mathf.InverseLerp(0f, MinRpm * 0.25f, Mathf.Min(_rpm, MinRpm * 0.25f));

                ApplyAudio(rpmNorm, 1f, volEnvelope);
                yield return null;
            }

            Sources[0].Stop();

            // Let base class finish any bookkeeping it needs.
            yield return base.Loop();
        }

        // === Helpers ===========================================================

        private float ComputeLoad(float dt)
        {
            var mem = Mathf.Clamp01(_memoryUsage);
            var frameLoad = 0f;

            if (IncludeFrameLoad)
            {
                var targetDt = 1f / Mathf.Max(1, TargetFps);
                var smoothedDt = Mathf.Lerp(targetDt, Time.smoothDeltaTime, 0.85f);
                frameLoad = Mathf.InverseLerp(targetDt * 0.9f, targetDt * 2.0f, smoothedDt);
                frameLoad = Mathf.Clamp01(frameLoad);
            }

            var noise = Mathf.PerlinNoise(_perlinSeed + 17.123f, Time.time * 0.35f);

            var wMem = MemoryWeight;
            var wFrame = IncludeFrameLoad ? FrameLoadWeight : 0f;
            var wNoise = NoiseWeight;

            var sum = wMem + wFrame + wNoise;
            if (sum > 1f)
            {
                wMem /= sum;
                wFrame /= sum;
                wNoise /= sum;
            }

            var load = (mem * wMem) + (frameLoad * wFrame) + (noise * wNoise);
            return Mathf.Clamp01(Mathf.Max(load, AmbientLoad));
        }

        private void UpdateThermal(float load, float dt)
        {
            var target = load;
            var tau = (target > _thermal) ? Mathf.Max(0.001f, HeatRiseTime)
                                          : Mathf.Max(0.001f, CoolFallTime);
            _thermal += (target - _thermal) * (dt / tau);
            _thermal = Mathf.Clamp01(_thermal);
        }

        private void ApplyAudio(float rpmNorm, float pitchMod, float volMod = 1f)
        {
            var basePitch = _pitchCurve.Evaluate(rpmNorm);
            var baseVol = _volumeCurve.Evaluate(rpmNorm);

            Sources[0].pitch = basePitch * pitchMod;
            Sources[0].volume = baseVol * volMod;
        }
    }
}
