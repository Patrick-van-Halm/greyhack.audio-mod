using System.Reflection;
using AudioMod.Unity;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using Utils.Logging;

namespace AudioMod
{
    [BepInPlugin("nl.pvanhalm.plugins.greyhack.audio-mod", "Audio Mod", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _initialized;
        internal static PluginLogger Logger;
        internal static PluginInfo Info;

        private void OnEnable()
        {
            if(_initialized) return;
            _initialized = true;
            Logger = new PluginLogger(base.Logger);
            Info = base.Info;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            
            new GameObject("AudioManager").AddComponent<AudioManager>();
            AudioLib.Load();
        }
    }
}