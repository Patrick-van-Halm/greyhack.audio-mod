using AudioMod.Handlers;
using HarmonyLib;
using TerminalPoolSystem;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(TerminalListAdapter), nameof(TerminalListAdapter.Delete))]
    public static class TerminalListAdapter_Delete
    {
        private static bool Prefix(TerminalListAdapter __instance)
        {
            return TerminalInputHandler.Delete(__instance);
        }
    }
}