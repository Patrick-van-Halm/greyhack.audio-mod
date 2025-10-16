using System.Collections;
using AudioMod.AudioSequence;
using HarmonyLib;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(Downloads), nameof(Downloads.DownloadProgress))]
    public class Download_DownloadProgress
    {
        static void Postfix(ref IEnumerator __result)
        {
            __result = Wrap(__result);
        }
        
        static IEnumerator Wrap(IEnumerator inner)
        {
            HardDisk.Instance.TransferWindowOpened();
            
            // run the original, step by step
            while (inner.MoveNext())
            {
                yield return inner.Current;
            }

            HardDisk.Instance.TransferWindowClosed();
        }
    }
}