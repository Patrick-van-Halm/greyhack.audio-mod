using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace AudioMod.Unity
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        protected override bool DontDestroyInstanceOnLoad => true;

        private static AudioType AudioTypeFromExtension(string extension) => extension.ToLower() switch
        {
            ".mp3" => AudioType.MPEG,
            ".mp4" => AudioType.MPEG,
            ".ogg" => AudioType.OGGVORBIS,
            ".wav" => AudioType.WAV,
        };

        public IEnumerator LoadAudioClipFromFile(string path, Action<AudioClip> callback)
        {
            var uriBuilder = new UriBuilder(path) { Scheme = Uri.UriSchemeFile };
            using var www =
                UnityWebRequestMultimedia.GetAudioClip(uriBuilder.ToString(), AudioTypeFromExtension(Path.GetExtension(path)));
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Plugin.Logger.LogError($"Could not load AudioClip from: {path}\nError: {www.error}\nStatus: {www.result}");
                yield break;
            }
            callback(DownloadHandlerAudioClip.GetContent(www));
            Plugin.Logger.LogInfo($"Successfully loaded AudioClip from: {path}");
        }

        public AudioSource CreateSource()
        {
            var obj = new GameObject();
            obj.transform.SetParent(transform);
            obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            return obj.AddComponent<AudioSource>();
        }
    }
}