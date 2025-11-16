using System.Linq;
using AudioMod.AudioSequence;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(RamWidget), nameof(RamWidget.Update))]
    public class RamWidget_Awake
    {
        private static bool Initialized;
        static void Prefix()
        {
            try
            {
                if (!PlayerClient.Singleton) return;
                if (PlayerClient.Singleton.player == null) return;
                if (PlayerClient.Singleton.player.pc == null) return;
                if (PlayerClient.Singleton.player.pc.procesos == null) return;
                if (Initialized) return;
                Initialized = true;
                var ramUsage = PlayerClient.Singleton.player.pc.procesos.Sum(process => process.ramUsedMb);
                var totalMemoryRam = PlayerClient.Singleton.player.pc.GetHardware().GetTotalMemoryRam();
                var percentUsage = ramUsage * 100f / totalMemoryRam * 0.01f;
                Fan.Instance.setMemoryUsage(percentUsage);
            }
            catch
            {
                Initialized = false;
            }
        }
    }
}