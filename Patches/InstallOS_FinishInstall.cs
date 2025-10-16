using AudioMod.AudioSequence;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(InstallOS), nameof(InstallOS.FinishInstall))]
    public class InstallOS_FinishInstall
    {
        static void Postfix()
        {
            HardDisk.Instance.TransferWindowClosed();
        }
    }
}