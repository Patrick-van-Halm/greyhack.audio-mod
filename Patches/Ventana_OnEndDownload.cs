using AudioMod.AudioSequence;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(Ventana), nameof(Ventana.OnEndDownload))]
    public static class Ventana_OnEndDownload
    {
        private static void Prefix(Hardware.HardDisk hardDisk, Ventana __instance)
        {
            var pc = PlayerClient.Singleton.player.pc;
            if (__instance.GetRemoteNetID() != pc.GetID()) return;
            pc.GetHardware().hardDisk = hardDisk;
            if (hardDisk.performance >= 100f) return;
            OSError.Instance.Start();
        }
    }
}