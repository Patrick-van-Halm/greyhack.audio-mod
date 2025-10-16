using AudioMod.AudioSequence;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(Ventana), nameof(Ventana.ShowHardwareWarning))]
    public static class Ventana_ShowHardwareWarning
    {
        private static void Prefix(Hardware hardware, PlayerComputer localPC)
        {
            if (hardware.powerSupply.GetAvailablePower(hardware) >= 1f &&
                hardware.GetLowestHealthCpus() >= 45f &&
                hardware.GetLowestHealthRam() >= 45f &&
                hardware.motherBoard.health >= 45f) return;
            
            OSError.Instance.Start();
        }
    }
}