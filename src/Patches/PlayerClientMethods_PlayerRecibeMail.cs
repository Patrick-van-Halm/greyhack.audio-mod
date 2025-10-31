using AudioMod.AudioSequence;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(PlayerClientMethods), nameof(PlayerClientMethods.PlayerRecibeMail), typeof(string), typeof(string), typeof(byte[]))]
    public class PlayerClientMethods_PlayerRecibeMail
    {
        static void Postfix()
        {
            Notification.Instance.Start();
        }
    }
}