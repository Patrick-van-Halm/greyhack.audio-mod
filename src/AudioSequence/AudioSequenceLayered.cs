using System.Collections;
using AudioMod.Unity;
using UnityEngine;
using Utils.Logging;

namespace AudioMod.AudioSequence
{
    public abstract class AudioSequenceLayered<T> where T : AudioSequenceLayered<T>, new()
    {
        protected abstract AudioClip _clip { get; }
        public static readonly T Instance = new();
        private bool isInitialized;
        private AudioManager _audioManager;

        protected void Init()
        {
            if (isInitialized) return;
            Plugin.Logger.LogDebug($"Init => {GetType().Name}");
            if (!_audioManager) _audioManager = AudioManager.Instance;
            isInitialized = true;
        }

        protected AudioSource GetSource()
        {
            var source = _audioManager.CreateSource();
            source.outputAudioMixerGroup = AudioLib.SfxAudioGroup;
            source.clip = _clip;
            source.playOnAwake = false;
            return source;
        }
        
        public virtual void Start()
        {
            Init();
            Start(GetSource());
        }

        protected void Start(AudioSource source)
        {
            Init();
            _audioManager.StartCoroutine(Loop(source));
        }

        protected virtual IEnumerator Loop(AudioSource source)
        {
            Plugin.Logger.LogDebug($"Cleanup Loop => {GetType().Name}");
            source.Stop();
            Object.Destroy(source.gameObject);
            yield break;
        }
    }
}