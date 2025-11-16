using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(SfxPlayer), nameof(SfxPlayer.PlayClip))]
    public static class SfxPlayer_PlayClip
    {
        private static bool Prefix(SfxPlayer.SfxID index, float volume = 1f)
        {
            switch (index)
            {
                case SfxPlayer.SfxID.BootBeep:
                case SfxPlayer.SfxID.PowerOn:
                case SfxPlayer.SfxID.StartUpChime:
                    return false;
                
                default:
                    return true;
            }
        }
    }
}