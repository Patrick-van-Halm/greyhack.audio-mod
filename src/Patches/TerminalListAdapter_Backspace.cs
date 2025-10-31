using AudioMod.Handlers;
using HarmonyLib;
using TerminalPoolSystem;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(TerminalListAdapter), nameof(TerminalListAdapter.BackSpace))]
    public static class TerminalListAdapter_Backspace
    {
        private static bool Prefix(TerminalListAdapter __instance)
        {
            return TerminalInputHandler.Backspace(__instance);
        }
    }
}