using UnityEngine;

namespace AudioMod.Unity
{
    [DefaultExecutionOrder(-1)]
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour, new()
    {
        public static T Instance { get; private set; }
        protected abstract bool DontDestroyInstanceOnLoad { get; }
        
        protected virtual void Awake()
        {
            var typedInstance = (this as T)!;
            if (Instance && Instance != typedInstance)
            {
                Destroy(this);
                return;
            }

            Instance = typedInstance;
            if(DontDestroyInstanceOnLoad) DontDestroyOnLoad(gameObject);
        }
    }
}