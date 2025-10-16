using System.Reflection;
using AudioMod.Unity;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace AudioMod
{
    [BepInPlugin("nl.pvanhalm.plugins.greyhack.audio-mod", "Audio Mod", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _initialized;
        internal static ManualLogSource Logger;
        internal static PluginInfo Info;

        private void OnEnable()
        {
            if(_initialized) return;
            _initialized = true;
            Logger = base.Logger;
            Info = base.Info;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            
            new GameObject("AudioManager").AddComponent<AudioManager>();
            AudioLib.Load();
        }
    }
}