using System.IO;
using System.Reflection;

using BaseMod.Core.Extensions;

using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;

using UnityEngine.Events;
using UnityEngine.SceneManagement;

using YC.GameplayMod.Mods;
using YC.GameplayMod.Plugins;

namespace YC.GameplayMod;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin {
    internal static new ManualLogSource Log;
    internal static string PluginAssets;
    internal static string PluginConfigs;
    internal static string PluginResources;
    internal static Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load() {
        // Plugin startup logic
        Log = base.Log;
        string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        PluginAssets = Path.Combine(baseDirectory, "Assets");
        PluginConfigs = Path.Combine(baseDirectory, "Configs");
        PluginResources = Path.Combine(baseDirectory, "Resources");

        RandomReverseMod.Load(Config);
        SexChoiceRealismMod.Load(Config);

        Harmony.PatchAll(typeof(BattleManagerPatch));
        Harmony.PatchAll(typeof(CharacterSexPatch));
        Harmony.PatchAll(typeof(SexSystemPatch));

        SceneManager.sceneLoaded += (UnityAction<Scene, LoadSceneMode>)OnSceneLoaded;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Log.Info($"Scene loaded: Name: {scene.name}, BuildIndex: {scene.buildIndex}");
        if (scene.buildIndex >= 2) {
        }
    }
}
