using AudioMod.AudioSequence;
using HarmonyLib;
using Util;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(OS), nameof(OS.ShowError))]
    public static class OS_ShowError
    {
        private static void Postfix(string msg)
        {
            OSError.Instance.Start();
        }
    }
}