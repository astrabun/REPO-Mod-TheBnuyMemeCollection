using System.Collections;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace TheBnuyMemeCollection.patches
{
    [HarmonyPatch(typeof(EnemyHiddenAnim), "Update")]
    internal class EnemyHiddenSoundPatch
    {
        private static readonly HashSet<EnemyHiddenAnim> patched = new();

        [HarmonyPostfix]
        private static void Postfix(EnemyHiddenAnim __instance)
        {
            if (patched.Contains(__instance))
                return;

            patched.Add(__instance);
            __instance.StartCoroutine(ReplaceHiddenSound(__instance));
        }

        private static IEnumerator ReplaceHiddenSound(EnemyHiddenAnim anim)
        {
            string pluginDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AudioClip");
            string newSoundPath = Path.Combine(pluginDir, "rholiday.ogg");
            string newSoundUri = "file://" + newSoundPath.Replace("\\", "/");

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(newSoundUri, AudioType.OGGVORBIS);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                TheBnuyMemeCollection.Logger.LogError("Failed to load replacement sound: " + www.error);
                yield break;
            }

            AudioClip newClip = DownloadHandlerAudioClip.GetContent(www);

            if (anim.soundPlayerMove != null && anim.soundPlayerMove.Sounds != null && anim.soundPlayerMove.Sounds.Length > 0)
            {
                anim.soundPlayerMove.Sounds[0] = newClip;
            }
            else
            {
                TheBnuyMemeCollection.Logger.LogWarning("Could not find soundPlayerMove or its Sounds[]");
            }
        }
    }
}
