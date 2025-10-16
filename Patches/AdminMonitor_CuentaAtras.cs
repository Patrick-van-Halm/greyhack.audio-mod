using System.Collections;
using AudioMod.Handlers;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(AdminMonitor), nameof(AdminMonitor.CuentaAtras))]
    public class AdminMonitor_CuentaAtras
    {
        private static bool Prefix(AdminMonitor.InfoMonitor infoMonitor, ref IEnumerator __result, AdminMonitor __instance)
        {
            __result = AdminMonitorHandler.CustomCoroutine(__instance, infoMonitor);
            return false;
        }
    }
}