using System.Collections;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace TheBnuyMemeCollection.patches
{
    [HarmonyPatch(typeof(ValuableBoombox))]
    [HarmonyPatch("Start")]
    internal class MuseumBoomboxIs365PartyGirlPatch
    {
        [HarmonyPostfix]
        private static void Postfix(ValuableBoombox __instance)
        {
            __instance.StartCoroutine(ReplaceBoomboxClip(__instance));
        }

        private static IEnumerator ReplaceBoomboxClip(ValuableBoombox boombox)
        {
            string pluginDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AudioClip");
            string partyGirlClipPath = Path.Combine(pluginDir, "boombox music partygirl.ogg");
            string fixedPartyGirlClipPathForUri = partyGirlClipPath.Replace("\\", "/");

            UnityWebRequest wwwmedia = UnityWebRequestMultimedia.GetAudioClip("file://" + fixedPartyGirlClipPathForUri, AudioType.OGGVORBIS);
            yield return wwwmedia.SendWebRequest();

            if (wwwmedia.result != UnityWebRequest.Result.Success)
            {
                TheBnuyMemeCollection.Logger.LogError("Failed to load AudioClip: " + wwwmedia.error);
                yield break;
            }

            AudioClip partyGirlClip = DownloadHandlerAudioClip.GetContent(wwwmedia);
            boombox.soundBoomboxMusic.Sounds[0] = partyGirlClip;
        }
    }
}
