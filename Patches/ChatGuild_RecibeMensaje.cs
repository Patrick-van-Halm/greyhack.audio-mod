using AudioMod.AudioSequence;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(ChatGuild), nameof(ChatGuild.RecibeMensaje), typeof(PlayerUtilsChat.ChatMessage))]
    public class ChatGuild_RecibeMensaje
    {
        static void Postfix()
        {
            Notification.Instance.Start();
        }
    }
}