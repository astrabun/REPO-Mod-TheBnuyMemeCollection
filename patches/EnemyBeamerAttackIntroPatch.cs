using System.Collections;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace TheBnuyMemeCollection.patches
{
    [HarmonyPatch(typeof(EnemyBeamerAnim), "Awake")]
    internal class EnemyBeamerAttackIntroPatch
    {
        [HarmonyPostfix]
        private static void Postfix(EnemyBeamerAnim __instance)
        {
            __instance.StartCoroutine(ReplaceBeamerSound(__instance));
        }

        private static IEnumerator ReplaceBeamerSound(EnemyBeamerAnim anim)
        {
            string pluginDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AudioClip");
            string newSoundPath = Path.Combine(pluginDir, "enemy beamer attack intro.ogg");
            string newSoundUri = "file://" + newSoundPath.Replace("\\", "/");

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(newSoundUri, AudioType.OGGVORBIS);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                TheBnuyMemeCollection.Logger.LogError("Failed to load replacement sound: " + www.error);
                yield break;
            }

            AudioClip newClip = DownloadHandlerAudioClip.GetContent(www);

            if (anim.soundAttackIntro != null && anim.soundAttackIntro.Sounds != null && anim.soundAttackIntro.Sounds.Length > 0)
            {
                anim.soundAttackIntro.Sounds[0] = newClip;
            }
            else
            {
                TheBnuyMemeCollection.Logger.LogWarning("Could not find SoundAttackIntro or its Sounds[]");
            }
        }
    }
}
