using System.Collections;
using UnityEngine;

namespace AudioMod.AudioSequence
{
    public class HardDisk : AudioSequence<HardDisk>
    {
        private readonly AnimationCurve _pitchCurve = AnimationCurve.EaseInOut(0, 0.75f, 1, 1.2f);
        private readonly AnimationCurve _volumeCurve = AnimationCurve.EaseInOut(0, 0.05f, 1, 0.12f);

        private const float SPEED_UP_TICK = 0.04f;

        private ushort transferCount = 0;     // transfers (T)
        private int explorerWindowCount = 0; // explorers  (E)

        public void TransferWindowOpened() { transferCount++; Plugin.Logger.LogDebug("HardDisk => Transfer started"); }
        public void TransferWindowClosed() { if (transferCount > 0) transferCount--; Plugin.Logger.LogDebug("HardDisk => Transfer ended"); }
        
        public void ExplorerWindowOpened() { explorerWindowCount++; Plugin.Logger.LogDebug("HardDisk => File explorer opened"); }
        public void ExplorerWindowClosed() { if (explorerWindowCount > 0) explorerWindowCount--; Plugin.Logger.LogDebug("HardDisk => File explorer closed"); }

        // Boot triggers (call these from your sim)
        public void BootStarted() { isBooting = true;  Plugin.Logger.LogDebug("HardDisk => Boot Started"); }
        public void BootEnded()   { isBooting = false; Plugin.Logger.LogDebug("HardDisk => Boot Ended");   }
        
        private float tauT = 2.0f;
        private float tauE = 5.0f;
        private float wT = 0.8f;
        private float wE = 0.5f;
        private float gammaLoud = 0.6f;
        private float alphaPitch = 0.75f;

        
        private float attackS = 0.08f;      // disk spin-up feel
        private float releaseS = 0.35f;     // disk spin-down feel
        private float changeWeight = 0.9f;  // rate-of-change boost
        private float changeHalfLifeS = 0.25f;
        
        private float bootAttackS = 0.15f;  // ramp in after BootStarted()
        private float bootReleaseS = 1f;  // ramp out after BootEnded()
        private float bootBoostGain = 1.2f; // added into activity before soft-clip
        private bool isBooting = false;                      // set by BootStarted/BootEnded
        private float bootEnv = 0f;                          // smoothed 0..1 boot envelope
        private float bootPitchWeight = 0.6f; // 0..1: how strongly boot pulls pitch upward
        private float bootPitchEase = 0.15f;    // 0..1: if >0, eases the boot pitch ramp (0 = linear)
        
        private float noise1Hz = 1.7f;  // volume wobble
        private float noise2Hz = 4.3f;  // pitch flutter
        private float noiseAmpV = 0.1f;
        private float noiseAmpP = 0.15f;

        // Internal state
        private float activitySmoothed = 0f;
        private float phase1 = 0f, phase2 = 0f;
        private float prevT = 0f, prevE = 0f;
        private float changeMem = 0f;

        protected override AudioClip[] _clips => new[]
        {
            AudioLib.HardDiskSpinUp,   // Source 0
            AudioLib.HardDiskSpinDown, // Source 1
            AudioLib.HardDiskLoop,     // Source 2 (loop, modulated)
        };

        public override void Start()
        {
            Init();
            
            // Fixed levels for non-loop sources; we never change these later
            Sources[0].volume = _volumeCurve.Evaluate(0); // spin-up
            Sources[1].volume = _volumeCurve.Evaluate(0); // spin-down

            // Loop source setup (modulated later)
            Sources[2].loop = true;
            Sources[2].pitch = _pitchCurve.Evaluate(0);
            Sources[2].volume = _volumeCurve.Evaluate(0);

            prevT = transferCount;
            prevE = Mathf.Max(0, explorerWindowCount);
            changeMem = 0f;
            bootEnv = 0f;
            
            base.Start();
        }

        private void ComputeNormalizedPV(float dt, out float p01, out float v01)
        {
            // Current counts
            var T = (float)transferCount;
            var E = (float)Mathf.Max(0, explorerWindowCount);

            // 1) Level (soft-saturated)
            var t = 1f - Mathf.Exp(-T / Mathf.Max(0.0001f, tauT));
            var e = 1f - Mathf.Exp(-E / Mathf.Max(0.0001f, tauE));

            // 2) Dynamics from rate-of-change (burstiness on rising edges)
            var dT = Mathf.Max(0f, (T - prevT) / Mathf.Max(0.0001f, dt));
            var dE = Mathf.Max(0f, (E - prevE) / Mathf.Max(0.0001f, dt));
            var dTn = 1f - Mathf.Exp(-dT / 3f); // ~3 dialogs/s => strong burst
            var dEn = 1f - Mathf.Exp(-dE / 6f); // ~6 explorers/s => strong burst
            var kChange = 1f - Mathf.Exp(-dt / Mathf.Max(0.0001f, changeHalfLifeS));
            var changeNow = Mathf.Clamp01(0.7f * dTn + 0.3f * dEn);
            changeMem += kChange * (changeNow - changeMem); // EMA memory

            // 3) Boot envelope (start/end only)
            var bootTarget = isBooting ? 1f : 0f;
            var kBoot = 1f - Mathf.Exp(-dt / Mathf.Max(0.0001f, isBooting ? bootAttackS : bootReleaseS));
            bootEnv += kBoot * (bootTarget - bootEnv);
            var bootBoost = bootBoostGain * Mathf.Clamp01(bootEnv);

            // 4) Combine and soft-clip to activity
            var levelTerm = wT * t + wE * e;
            var dynamicTerm = changeWeight * changeMem;
            var aRaw = 1f - Mathf.Exp(-(levelTerm + dynamicTerm + bootBoost));

            // 5) Attack/Release envelope for overall disk feel
            var kAtt = 1f - Mathf.Exp(-dt / Mathf.Max(0.0001f, attackS));
            var kRel = 1f - Mathf.Exp(-dt / Mathf.Max(0.0001f, releaseS));
            activitySmoothed += (aRaw > activitySmoothed ? kAtt : kRel) * (aRaw - activitySmoothed);
            var A = Mathf.Clamp01(activitySmoothed);

            // 6) LFOs
            phase1 = (phase1 + noise1Hz * dt) % 1f;
            phase2 = (phase2 + noise2Hz * dt) % 1f;
            var lfo1 = Mathf.Sin(2f * Mathf.PI * phase1);
            var lfo2 = 0.6f * Mathf.Sin(2f * Mathf.PI * phase2) + 0.4f * (Random.value * 2f - 1f);

            // 7) Volume & Pitch (normalized 0..1)
            var baseV = Mathf.Pow(A, gammaLoud);
            v01 = Mathf.Clamp01(baseV + noiseAmpV * lfo1);

            // --- Pitch (normalized 0..1) ---
            // Base from counts: more transfers -> higher pitch; explorers add via sqrt
            var pBase = Mathf.Clamp01(alphaPitch * t + (1f - alphaPitch) * Mathf.Sqrt(e));

            // Boot pitch term with adjustable easing amount (0..1)
            var bootLin   = Mathf.Clamp01(bootEnv);
            var bootEaseD = Mathf.SmoothStep(0f, 1f, bootLin);
            var pBoot     = Mathf.Lerp(bootLin, bootEaseD, Mathf.Clamp01(bootPitchEase)); // 0=linear, 1=eased

            // Mix: during boot, pBoot pulls pitch toward 1.0 with weight bootPitchWeight
            var pMixed = Mathf.Clamp01(Mathf.Lerp(pBase, pBoot, Mathf.Clamp01(bootPitchWeight)));

            // Keep a *small* dependence on overall activity so heavy load still nudges pitch
            var pitchActivityCoupling = 0.15f; // was hard-coded as 0.15
            p01 = Mathf.Clamp01(pMixed * (0.85f + pitchActivityCoupling * A) + noiseAmpP * lfo2);

            // history
            prevT = T;
            prevE = E;
        }

        // Apply modulation only to the loop source, and only if it is playing
        private void ApplyPitchAndVolume(float pitch, float volume)
        {
            foreach (var s in Sources)
            {
                s.pitch = pitch;
                s.volume = volume;
            }
            // MelonLogger.Msg($"Pitch: {pitch:F3} | Vol: {volume:F3}");
        }

        private IEnumerator ModulatePitchAndVolume()
        {
            var pitch = _pitchCurve.Evaluate(0);
            while (true)
            {
                yield return null;
                ComputeNormalizedPV(Time.deltaTime, out var p01, out var v01);

                var desiredPitch = _pitchCurve.Evaluate(p01);
                var desiredVol = _volumeCurve.Evaluate(v01);

                if (!Mathf.Approximately(pitch, desiredPitch))
                {
                    var step = SPEED_UP_TICK * Time.deltaTime;
                    var nextPitch = Mathf.MoveTowards(pitch, desiredPitch, step);
                    pitch = nextPitch;
                }

                ApplyPitchAndVolume(pitch, desiredVol);
            }
        }

        protected override IEnumerator Loop()
        {
            var modulatorRoutine = _audioManager.StartCoroutine(ModulatePitchAndVolume());
            
            // --- Spin-up phase: NO modulation yet; loop not started ---
            Sources[0].Play();
            yield return new WaitUntil(() => !Sources[0].isPlaying || isStopping);

            if (!isStopping)
            {
                // --- Start looping noise after spin-up finishes ---
                Sources[2].Play();

                // --- Running: modulate only the loop source ---
                while (!isStopping)
                {
                    yield return null;
                }

                // --- Shutdown: stop the loop BEFORE playing spin-down ---
                Sources[2].Stop(); // ensures no overlap with spin-down clip
            }
            transferCount = 0;
            explorerWindowCount = 0;

            // --- Spin-down phase: loop is stopped, so no modulation occurs ---
            Sources[1].Play();
            yield return new WaitUntil(() => !Sources[1].isPlaying);
            
            _audioManager.StopCoroutine(modulatorRoutine);
            // Hand back to base
            yield return base.Loop();
        }
    }
}
