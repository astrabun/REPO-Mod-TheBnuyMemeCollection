using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace TheBnuyMemeCollection.patches
{
    public class ShopMusicRunner : MonoBehaviour
    {
        private bool musicReplaced = false;
        private AudioClip? replacementClip;

        private void Start()
        {
            TheBnuyMemeCollection.Logger.LogInfo("ShopMusicRunner.Start()");
            StartCoroutine(LoadReplacementClip());
        }

        private IEnumerator LoadReplacementClip()
        {
            string pluginDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string path = Path.Combine(pluginDir, "AudioClip", "shophoopdreams.ogg").Replace("\\", "/");

            using UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.OGGVORBIS);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                TheBnuyMemeCollection.Logger.LogError("Failed to load replacement music: " + req.error);
                yield break;
            }

            replacementClip = DownloadHandlerAudioClip.GetContent(req);
            replacementClip.name = "shophoopdreams";
            TheBnuyMemeCollection.Logger.LogInfo("Replacement music loaded");
        }

        private void Update()
        {
            if (musicReplaced || replacementClip == null) return;

            foreach (var src in Resources.FindObjectsOfTypeAll<AudioSource>())
            {
                if (src.clip != null && src.clip.name.ToLower().Contains("shop"))
                {
                    bool wasPlaying = src.isPlaying;
                    src.clip = replacementClip;
                    if (wasPlaying) src.Play();

                    TheBnuyMemeCollection.Logger.LogInfo($"Replaced shop music on: {src.gameObject.name}");
                    musicReplaced = true;
                    break;
                }
            }
        }
    }
}
