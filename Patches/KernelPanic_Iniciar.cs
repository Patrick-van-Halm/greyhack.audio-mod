using AudioMod.AudioSequence;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(KernelPanic), nameof(KernelPanic.Iniciar))]
    public class KernelPanic_Iniciar
    {
        private static void Postfix()
        {
            SystemFailure.Instance.Start();
        }
    }
}