using System.Collections;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace TheBnuyMemeCollection.patches
{
    [HarmonyPatch(typeof(EnemyHunterAnim), "Awake")]
    internal class EnemyHunterHummingLoopPatch
    {
        [HarmonyPostfix]
        private static void Postfix(EnemyHunterAnim __instance)
        {
            __instance.StartCoroutine(ReplaceSound(__instance));
        }

        private static IEnumerator ReplaceSound(EnemyHunterAnim anim)
        {
            string pluginDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AudioClip");
            string newSoundPath = Path.Combine(pluginDir, "enemy hunter nutting loop.ogg");
            string newSoundUri = "file://" + newSoundPath.Replace("\\", "/");

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(newSoundUri, AudioType.OGGVORBIS);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                TheBnuyMemeCollection.Logger.LogError("Failed to load replacement sound: " + www.error);
                yield break;
            }

            AudioClip newClip = DownloadHandlerAudioClip.GetContent(www);

            if (anim.soundHumming != null && anim.soundHumming.Sounds != null && anim.soundHumming.Sounds.Length > 0)
            {
                anim.soundHumming.Sounds[0] = newClip;
            }
            else
            {
                TheBnuyMemeCollection.Logger.LogWarning("Could not find soundHumming or its Sounds[]");
            }
        }
    }
}
