using System;
using System.Collections;
using System.Collections.Generic;
using AudioMod.AudioSequence;
using UnityEngine;

namespace AudioMod.Handlers
{
    public static class AdminMonitorHandler
    {
        private static AdminMonitor.InfoMonitor? Tracking = null;
        private static List<AdminMonitor.InfoMonitor> AllTraces = new();
        private const float InitialTraceTime = 15f;

        private static void OnStartTrace(AdminMonitor.InfoMonitor infoMonitor)
        {
            if(!AllTraces.Contains(infoMonitor)) AllTraces.Add(infoMonitor);
            if(Tracking != null && infoMonitor.tiempoRestante >= Tracking.tiempoRestante) return;
            Tracking = infoMonitor;
            
            if (TraceBeep.Instance.IsRunning) return;
            TraceBeep.Instance.Detection = 0;
            TraceBeep.Instance.Start();
        } 
        
        public static IEnumerator CustomCoroutine(AdminMonitor __instance, AdminMonitor.InfoMonitor infoMonitor)
        {
            OnStartTrace(infoMonitor);
            __instance.panelClock.SetActive(true);
            __instance.StartBlinking();
            float? updateDisplayTick = null;
            while (infoMonitor.tiempoRestante > 0f)
            {
                var targetDisplayTickTime = infoMonitor.tiempoRestante <= 10f || __instance.activeTraces.Count <= 1 ? 1 : 10;
                if (updateDisplayTick == null || updateDisplayTick >= targetDisplayTickTime )
                {
                    updateDisplayTick = 0f;
                    if (__instance.activeTraces.Count <= 1)
                    {
                        var timeSpan = TimeSpan.FromSeconds((int)infoMonitor.tiempoRestante);
                        __instance.clockLabel.text = timeSpan.TotalHours >= 1.0 
                            ? $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}"
                            : $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                    }
                    else
                    {
                        var text = "";
                        for (var i = 0; i < __instance.activeTraces.Count; i++)
                        {
                            text += "! ";
                        }
                        __instance.clockLabel.text = text;
                        __instance.AddTexto($"<color=\"red\">[!] {infoMonitor.remoteComputerIp.CensorAddress()} : Active trace {infoMonitor.tiempoRestante:N0} seconds left.</color>\n");
                    }
                }
                yield return null;
                updateDisplayTick += Time.deltaTime;
                infoMonitor.tiempoRestante -= Time.deltaTime;
                if (Tracking == infoMonitor) TraceBeep.Instance.Detection = InitialTraceTime > 0 ? 1 - infoMonitor.tiempoRestante / InitialTraceTime : 0;
            }
            __instance.AddTexto($"<color=\"red\">[!] {infoMonitor.remoteComputerIp.CensorAddress()} : Active Trace completed.</color>\n");
            __instance.activeTraces.Remove(infoMonitor.remoteComputerID);
            CancelTrace(infoMonitor.remoteComputerID);
        }

        public static void CancelTrace(string remoteComputerID)
        {
            var traceInfoIdx = AllTraces.FindIndex(trace => trace.remoteComputerID == remoteComputerID);
            if (traceInfoIdx >= 0) AllTraces.RemoveAt(traceInfoIdx);
            if (Tracking?.remoteComputerID == remoteComputerID) Tracking = null;
            if (AllTraces.Count == 0)
            {
                if(TraceBeep.Instance.IsRunning) TraceBeep.Instance.Stop();
                return;
            }

            Tracking = AllTraces[0];
            for (var i = 1; i < AllTraces.Count; i++)
            {
                if(AllTraces[i].tiempoRestante >= Tracking.tiempoRestante) continue;
                Tracking = AllTraces[i];
            }
        }
    }
}