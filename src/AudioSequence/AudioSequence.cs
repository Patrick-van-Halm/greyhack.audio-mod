using System.Collections;
using System.Collections.Generic;
using AudioMod.Unity;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioMod.AudioSequence
{
    public abstract class AudioSequence<T> where T : AudioSequence<T>, new()
    {
        public static readonly T Instance = new();

        public bool IsRunning => _loopRoutine != null;
        
        protected bool isStopping;
        private bool isInitialized;
        protected abstract AudioClip[] _clips { get; }
        
        protected AudioManager _audioManager;
        private Coroutine? _loopRoutine = null;
        private readonly List<AudioSource> _sources = new();
        protected IReadOnlyList<AudioSource> Sources => _sources;

        protected virtual void Init()
        {
            isStopping = false;
            if (isInitialized) return;
            Plugin.Logger.LogDebug($"Init => {GetType().Name}");
            if (!_audioManager) _audioManager = AudioManager.Instance;
            if (_sources.Count != 0) return;
            
            _sources.Clear();
            foreach (var clip in _clips)
            {
                var src = _audioManager.CreateSource();
                src.outputAudioMixerGroup = AudioLib.SfxAudioGroup;
                src.clip = clip;
                src.playOnAwake = false;
                _sources.Add(src);
            }

            isInitialized = true;
        }
        
        public virtual void Start()
        {
            Init();
            _loopRoutine = _audioManager.StartCoroutine(Loop());
        }

        public virtual void Stop(bool immediate = false)
        {
            if (!IsRunning) return;
            if (immediate)
            {
                Plugin.Logger.LogDebug($"Stopping Loop Immediately => {GetType().Name}");
                _audioManager.StopCoroutine(_loopRoutine);
                foreach (var src in _sources) src.Stop();
                return;
            }
            
            isStopping = true;
            Plugin.Logger.LogDebug($"Stopping Loop => {GetType().Name}");
        }

        protected virtual IEnumerator Loop()
        {
            Plugin.Logger.LogDebug($"Cleanup Loop => {GetType().Name}");
            foreach (var source in Sources)
            {
                source.Stop();
            }
            _loopRoutine = null;
            yield break;
        }
    }
}