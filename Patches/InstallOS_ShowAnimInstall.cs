using AudioMod.AudioSequence;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(InstallOS), nameof(InstallOS.ShowAnimInstall), typeof(string), typeof(string), typeof(string))]
    public class InstallOS_ShowAnimInstall
    {
        static void Postfix()
        {
            HardDisk.Instance.TransferWindowOpened();
        }
    }
}