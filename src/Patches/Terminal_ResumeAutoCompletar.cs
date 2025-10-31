using AudioMod.Handlers;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(Terminal), nameof(Terminal.ResumeAutoCompletar))]
    public static class Terminal_ResumeAutoCompletar
    {
        private static bool Prefix(byte[]? zipOutput, bool listFiles)
        {
            return TerminalInputHandler.AutoComplete(zipOutput, listFiles);
        }
    }
}