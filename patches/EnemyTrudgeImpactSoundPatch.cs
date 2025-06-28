using System;
using System.Collections;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace TheBnuyMemeCollection.patches
{
    [HarmonyPatch(typeof(EnemySlowWalkerAnim), "Awake")]
    internal class EnemyTrudgeImpactSoundPatch
    {
        [HarmonyPostfix]
        private static void Postfix(EnemySlowWalkerAnim __instance)
        {
            __instance.StartCoroutine(ReplaceSound(__instance));
        }

        private static IEnumerator ReplaceSound(EnemySlowWalkerAnim anim)
        {
            string pluginDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AudioClip");
            string newSoundPath = Path.Combine(pluginDir, "VineBoom.ogg");
            string newSoundUri = "file://" + newSoundPath.Replace("\\", "/");

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(newSoundUri, AudioType.OGGVORBIS);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                TheBnuyMemeCollection.Logger.LogError("Failed to load replacement sound: " + www.error);
                yield break;
            }

            AudioClip newClip = DownloadHandlerAudioClip.GetContent(www);

            Array soundsToChange = new Sound[] { anim.sfxFootstepSmall, anim.sfxFootstepBig, anim.sfxAttackImplosionImpactLocal, anim.sfxAttackImplosionHitLocal };
            foreach (Sound sound in soundsToChange)
            {
                if (sound != null && sound.Sounds != null && sound.Sounds.Length > 0)
                {
                    sound.Sounds[0] = newClip;
                }
                else
                {
                    TheBnuyMemeCollection.Logger.LogWarning($"Could not find {sound?.Source.name} or its Sounds[]");
                }
            }
        }
    }
}
