using System.Collections;
using UnityEngine;

namespace AudioMod.AudioSequence
{
    public class Notification : AudioSequenceLayered<Notification>
    {
        protected override AudioClip _clip => AudioLib.OSNotification;

        public override void Start()
        {
            Init();
            var src = GetSource();
            src.volume = .3f;
            base.Start(src);
        }

        protected override IEnumerator Loop(AudioSource source)
        {
            source.Play();
            yield return new WaitUntil(() => !source.isPlaying);
            yield return base.Loop(source);
        }
    }
}