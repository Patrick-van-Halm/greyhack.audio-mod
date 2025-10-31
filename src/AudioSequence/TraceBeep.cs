using System.Collections;
using UnityEngine;

namespace AudioMod.AudioSequence
{
    public class TraceBeep : AudioSequence<TraceBeep>
    {
        private readonly AnimationCurve _pitchCurve = AnimationCurve.EaseInOut(0f, .5f, 1f, 2f);
        public float Detection { get; set; } = 0f;
        
        protected override AudioClip[] _clips => new[]
        {
            AudioLib.TraceBeep
        };

        public override void Start()
        {
            Init();
            Sources[0].loop = true;
            Sources[0].volume = .3f;
            Sources[0].pitch = _pitchCurve.Evaluate(Detection);
            base.Start();
        }

        protected override IEnumerator Loop()
        {
            Sources[0].Play();

            while (!isStopping)
            {
                Sources[0].pitch = _pitchCurve.Evaluate(Detection);
                yield return null;
            }
            
            yield return base.Loop();
        }
    }
}