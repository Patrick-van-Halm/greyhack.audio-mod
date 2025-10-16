using System.Collections;
using AudioMod.AudioSequence;
using HarmonyLib;
using UnityEngine;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(BootUp), nameof(BootUp.AnimScreen))]
    public class BootUp_AnimScreen
    {
        static void Postfix(ref IEnumerator __result)
        {
            __result = Wrap(__result);
        }
    
        static IEnumerator Wrap(IEnumerator inner)
        {
            if (Bootup.Booted)
            {
                yield return new WaitUntil(() => !Fan.Instance.IsRunning && !HardDisk.Instance.IsRunning);
                Bootup.Booted = false;
            }
            
            Bootup.Instance.Start();
        
            // run the original, step by step
            while (inner.MoveNext())
            {
                yield return inner.Current;
            }

            Bootup.Booted = true;
        }
    }
}