using System.Collections.Generic;
using System.Linq;
using AudioMod.AudioSequence;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(RamWidget), nameof(RamWidget.UpdateProcesos), typeof(List<Computer.Proceso>))]
    public class RamWidget_UpdateProcesos
    {
        static void Prefix(List<Computer.Proceso> origProcesos)
        {
            var ramUsage = origProcesos.Sum(process => process.ramUsedMb);
            var totalMemoryRam = PlayerClient.Singleton.player.pc.GetHardware().GetTotalMemoryRam();
            var percentUsage = ramUsage * 100f / totalMemoryRam * 0.01f;
            Fan.Instance.setMemoryUsage(percentUsage);
        }
    }
}