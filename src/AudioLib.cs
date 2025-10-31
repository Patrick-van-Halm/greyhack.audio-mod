using System.IO;
using AudioMod.Unity;
using UnityEngine;

namespace AudioMod
{
    public class AudioLib
    {
        private static string AudioDirectory = Path.Combine(Path.GetDirectoryName(Plugin.Info.Location), "Audio");

        public static AudioClip BootupBeep;
        public static AudioClip HardDiskSpinUp;
        public static AudioClip HardDiskLoop;
        public static AudioClip HardDiskSpinDown;
        public static AudioClip OSBoot;
        public static AudioClip OSNotification;
        public static AudioClip OSActionFail;
        public static AudioClip OSError;
        public static AudioClip CPUFanLoop;
        public static AudioClip TraceBeep;
        public static AudioClip SystemFailureLoop;
        
        public static void Load()
        {
            if(!AudioManager.Instance) return;
            var audioManager = AudioManager.Instance;
            audioManager.StartCoroutine(audioManager.LoadAudioClipFromFile(Path.Combine(AudioDirectory, "bootup-beep.wav"), clip => BootupBeep = clip));
            audioManager.StartCoroutine(audioManager.LoadAudioClipFromFile(Path.Combine(AudioDirectory, "harddrive-spinup.wav"), clip => HardDiskSpinUp = clip));
            audioManager.StartCoroutine(audioManager.LoadAudioClipFromFile(Path.Combine(AudioDirectory, "harddrive-loop.wav"), clip => HardDiskLoop = clip));
            audioManager.StartCoroutine(audioManager.LoadAudioClipFromFile(Path.Combine(AudioDirectory, "harddrive-spindown.wav"), clip => HardDiskSpinDown = clip));
            audioManager.StartCoroutine(audioManager.LoadAudioClipFromFile(Path.Combine(AudioDirectory, "os-boot.wav"), clip => OSBoot = clip));
            audioManager.StartCoroutine(audioManager.LoadAudioClipFromFile(Path.Combine(AudioDirectory, "os-notification.wav"), clip => OSNotification = clip));
            audioManager.StartCoroutine(audioManager.LoadAudioClipFromFile(Path.Combine(AudioDirectory, "os-action-fail.wav"), clip => OSActionFail = clip));
            audioManager.StartCoroutine(audioManager.LoadAudioClipFromFile(Path.Combine(AudioDirectory, "os-error.wav"), clip => OSError = clip));
            audioManager.StartCoroutine(audioManager.LoadAudioClipFromFile(Path.Combine(AudioDirectory, "cpu-fan-loop.wav"), clip => CPUFanLoop = clip));
            audioManager.StartCoroutine(audioManager.LoadAudioClipFromFile(Path.Combine(AudioDirectory, "trace-beep.wav"), clip => TraceBeep = clip));
            audioManager.StartCoroutine(audioManager.LoadAudioClipFromFile(Path.Combine(AudioDirectory, "system-failure-loop.wav"), clip => SystemFailureLoop = clip));
        }
    }
}