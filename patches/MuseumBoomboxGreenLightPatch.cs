using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace TheBnuyMemeCollection.patches
{
    [HarmonyPatch(typeof(ValuableBoombox), "Start")]
    internal class ForceUniqueMaterialPatch
    {
        [HarmonyPostfix]
        private static void Postfix(ValuableBoombox __instance)
        {
            // This clones the materials so they're no longer shared
            Renderer rend1 = __instance.speaker1.GetComponent<Renderer>();
            Renderer rend2 = __instance.speaker2.GetComponent<Renderer>();
            rend1.material = new Material(rend1.material);
            rend2.material = new Material(rend2.material);
        }
    }

    [HarmonyPatch(typeof(ValuableBoombox), "Update")]
    internal class MuseumBoomboxGreenLightPatch
    {
        private static readonly FieldInfo PhysGrabField = typeof(Trap).GetField("physGrabObject", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly Color Green = new(0.541f, 0.808f, 0f);
        private static readonly Color White = Color.white;
        private static readonly Color Black = Color.black;
        private static float[] _spectrum = new float[64]; // Lower values = lower frequencies

        [HarmonyPostfix]
        private static void Postfix(ValuableBoombox __instance)
        {
            if (PhysGrabField?.GetValue(__instance) is not PhysGrabObject physGrab) return;
            if (!physGrab.grabbed) return;

            // Grab the AudioSource from the Sound object
            var source = __instance.soundBoomboxMusic.Source;
            if (source == null || !source.isPlaying) return;

            source.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

            // Get low-frequency energy (bins 1–6)
            float bass = 0f;
            for (int i = 1; i <= 6; i++)
            {
                bass += _spectrum[i];
            }
            bass /= 6f;

            // Map bass to a 0–1 scale, then remap to strobe color
            float strength = Mathf.Clamp01(bass * 20f); // Adjust multiplier for reactivity

            // strength near 0 = black, near 1 = green, mid = white
            Color targetColor;
            if (strength < 0.3f)
                targetColor = Color.Lerp(Black, White, strength / 0.3f); // black -> white
            else
                targetColor = Color.Lerp(White, Green, (strength - 0.3f) / 0.7f); // white -> green

            var mat1 = __instance.speaker1.GetComponent<Renderer>().material;
            var mat2 = __instance.speaker2.GetComponent<Renderer>().material;

            mat1.SetColor("_EmissionColor", targetColor);
            mat2.SetColor("_EmissionColor", targetColor);

            __instance.light1.color = targetColor;
            __instance.light2.color = targetColor;
            __instance.light1.intensity = Mathf.Lerp(__instance.light1.intensity, 4f, Time.deltaTime * 8f);
            __instance.light2.intensity = Mathf.Lerp(__instance.light2.intensity, 4f, Time.deltaTime * 8f);
        }
    }

}
