using System.Collections;
using AudioMod.AudioSequence;
using HarmonyLib;
using UnityEngine;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(BootUp), nameof(BootUp.InterruptBoot))]
    public class BootUp_InterruptBoot
    {
        static void Prefix()
        {
            Fan.Instance.Stop();
            HardDisk.Instance.Stop();
        }
    }
}