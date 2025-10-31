using AudioMod.Handlers;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(AdminMonitor), nameof(AdminMonitor.CancelCuentaAtras))]
    public class AdminMonitor_CancelCuentaAtras
    {
        private static void Postfix(string ipRemotePc, string remoteComputerID)
        {
            AdminMonitorHandler.CancelTrace(remoteComputerID);
        }
    }
}