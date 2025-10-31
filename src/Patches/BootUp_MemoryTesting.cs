using System;
using System.Collections;
using AudioMod.AudioSequence;
using HarmonyLib;
using UnityEngine;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(BootUp), nameof(BootUp.MemoryTesting))]
    public class BootUp_MemoryTesting
    {
        private static bool Prefix(ref IEnumerator __result, BootUp __instance)
        {
            __result = Wrap(__instance);
            return false;
        }

        private static IEnumerator Wrap(BootUp instance)
        {
            var maxNum = instance.pc.GetHardware() != null 
                ? instance.pc.GetHardware().GetTotalMemoryRam() * 1024 
                : 131072;
            
            var incremento = (int)(maxNum * 0.015625f);
            var indiceTest = instance.textoScreen.text.IndexOf("Testing: ", StringComparison.Ordinal) + "Testing: ".Length;
            var i = 0;
            while (i <= maxNum)
            {
                var text = i.ToString();
                instance.textoScreen.text = instance.textoScreen.text[..indiceTest] + text + "K";
                i += incremento;
                yield return null;
            }

            instance.textoScreen.text += " OK"; 
            Bootup.Instance.MotherboardPostBeep();
            yield return new WaitForSeconds(0.65f);
        }
    }
}