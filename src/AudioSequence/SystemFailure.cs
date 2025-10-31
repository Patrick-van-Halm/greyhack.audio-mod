using System.Collections;
using UnityEngine;

namespace AudioMod.AudioSequence
{
    public class SystemFailure : AudioSequence<SystemFailure>
    {
        protected override AudioClip[] _clips => new[]
        {
            AudioLib.SystemFailureLoop
        };

        public override void Start()
        {
            Init();
            Sources[0].loop = true;
            Sources[0].volume = .3f;
            base.Start();
        }

        protected override IEnumerator Loop()
        {
            Sources[0].Play();
            while (!isStopping)
            {
                yield return null;
            }
            yield return base.Loop();
        }
    }
}