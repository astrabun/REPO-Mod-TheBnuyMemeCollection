using TheBnuyMemeCollection.patches;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace TheBnuyMemeCollection;

[BepInPlugin("AstraBun.TheBnuyMemeCollection", "TheBnuyMemeCollection", "1.1.0")]
public class TheBnuyMemeCollection : BaseUnityPlugin
{
    internal static TheBnuyMemeCollection Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    private void Awake()
    {
        Instance = this;
        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        Patch();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        Logger.LogInfo($"{Info.Metadata.GUID} Prepare for bunny memes heheheheheheh");

        gameObject.AddComponent<ShopMusicRunner>();
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll(typeof(MuseumBoomboxIs365PartyGirlPatch));
        Harmony.PatchAll(typeof(MuseumBoomboxGreenLightPatch));
        Harmony.PatchAll(typeof(EnemyBeamerAttackIntroPatch));
        Harmony.PatchAll(typeof(EnemyHunterHummingLoopPatch));
        Harmony.PatchAll(typeof(EnemyTrudgeImpactSoundPatch));
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }

    private void Update()
    {
        // Code that runs every frame goes here
    }
}