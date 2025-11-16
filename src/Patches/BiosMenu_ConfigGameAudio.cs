using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(BiosMenu), nameof(BiosMenu.ConfigGameAudio))]
    public static class BiosMenu_ConfigGameAudio
    {
        private static void Postfix(BiosMenu __instance)
        {
            AudioLib.Mixer = __instance.gameMixer;
        }
    }
}