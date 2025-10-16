using System.Collections;
using UnityEngine;

namespace AudioMod.AudioSequence
{
    public class Bootup : AudioSequence<Bootup>
    {
        public static bool Booted;

        protected override AudioClip[] _clips => new[]
        {
            AudioLib.BootupBeep,
            AudioLib.OSBoot,
        };

        public override void Start()
        {
            Init();
            Booted = false;

            Sources[0].volume = .1f;
            Sources[1].volume = .5f;

            base.Start();
            HardDisk.Instance.Start();
            Fan.Instance.Start();
        }

        protected override IEnumerator Loop()
        {
            HardDisk.Instance.BootStarted();
            yield return new WaitUntil(() => Booted);
            HardDisk.Instance.BootEnded();
            Sources[1].Play();
            yield return new WaitUntil(() => !Sources[1].isPlaying);
            yield return base.Loop();
        }

        public void MotherboardPostBeep()
        {
            Sources[0].Play();
        }
    }
}