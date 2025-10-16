using System.Collections;
using AudioMod.AudioSequence;
using HarmonyLib;
using UnityEngine;

namespace AudioMod.Patches
{
    [HarmonyPatch(typeof(Terminal), nameof(Terminal.ClosingPrograms))]
    public class Terminal_ClosingPrograms
    {
        private static void Postfix(bool exitGame, ref IEnumerator __result, Terminal __instance)
        {
            __result = Wrap(__instance, exitGame);
        }

        private static IEnumerator Wrap(Terminal instance, bool exitGame)
        {
            var programas = instance.transform.root.GetComponentsInChildren<Ventana>();
            int num;
            for (var i = 0; i < programas.Length; i = num + 1)
            {
                if (programas[i] != instance)
                {
                    if (programas[i] as Welcome != null || programas[i] as AdvancedTutorial != null)
                    {
                        programas[i].CloseTaskBar(true);
                    }
                    else
                    {
                        programas[i].LinkTutorial(null);
                        programas[i].CloseTaskBar();
                    }
                    yield return new WaitForSeconds(0.2f);
                }
                num = i;
            }
            yield return new WaitForSeconds(0.05f);
            instance.CloseTaskBar();
            yield return new WaitForSeconds(0.25f);
            
            SystemFailure.Instance.Stop();
            Fan.Instance.Stop();
            HardDisk.Instance.Stop();

            if (!exitGame) yield break;
            var bootup = instance.transform.root.GetComponentInChildren<BootUp>(true);
            bootup.gameObject.SetActive(true);
            Cursor.visible = false;
            bootup.fondoNegro.enabled = true;
            bootup.textoScreen.text = "";
            bootup.ActivarImagenes(false);
            bootup.transform.SetAsLastSibling();
            var component = GameObject.Find("TaskBar")?.GetComponent<Canvas>();
            if (component != null) component.sortingOrder = 0;

            yield return new WaitUntil(() => 
                !Fan.Instance.IsRunning && 
                !HardDisk.Instance.IsRunning && 
                !SystemFailure.Instance.IsRunning
            );
            Application.Quit();
        }
    }
}