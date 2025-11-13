using System.IO;

using BaseMod.Core.Logger;

using MelonLoader;
using MelonLoader.Utils;

using YC.GameplayMod;
using YC.GameplayMod.Configs;
using YC.GameplayMod.Mods;
using YC.GameplayMod.Patches;

[assembly: MelonInfo(typeof(Plugin), MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION, MyPluginInfo.PLUGIN_AUTHORS)]
[assembly: MelonGame(null, null)]

namespace YC.GameplayMod;
public class Plugin : MelonMod {
    internal static IPluginLogger Log;
    internal static string PluginAssets;
    internal static string PluginConfigs;
    internal static string PluginResources;

    public override void OnInitializeMelon() {
        base.OnInitializeMelon();

        // Plugin startup logic
        Log = new MelonPluginLogger( new MelonLogger.Instance(MyPluginInfo.PLUGIN_GUID));
        PluginAssets = Path.Combine(MelonEnvironment.UserDataDirectory, MyPluginInfo.PLUGIN_GUID, "Assets");
        PluginConfigs = Path.Combine(MelonEnvironment.UserDataDirectory, MyPluginInfo.PLUGIN_GUID, "Configs");
        PluginResources = Path.Combine(MelonEnvironment.UserDataDirectory, MyPluginInfo.PLUGIN_GUID, "Resources");

        var config = ModConfig.Load(Path.Combine( MelonEnvironment.UserDataDirectory, $"{MyPluginInfo.PLUGIN_GUID}.cfg") );

        DickStraponVisibilityMod.Load(config);
        RandomReverseMod.Load(config);
        SexChoiceRealismMod.Load(config);

        HarmonyInstance.PatchAll(typeof(BattleManagerPatch));
        HarmonyInstance.PatchAll(typeof(CharacterSexPatch));
        HarmonyInstance.PatchAll(typeof(SexEncounterPatch));

        Log.Info($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
        Log.Info($"Scene loaded: Name: {sceneName}, BuildIndex: {buildIndex}");
        if (buildIndex >= 2) {
            SexChoiceRealismMod.Prepare();
        }

        base.OnSceneWasLoaded(buildIndex, sceneName);
    }
}
/*
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

        DickStraponVisibilityMod.Load(Config);
        RandomReverseMod.Load(Config);
        SexChoiceRealismMod.Load(Config);

        Harmony.PatchAll(typeof(BattleManagerPatch));
        Harmony.PatchAll(typeof(CharacterSexPatch));
        Harmony.PatchAll(typeof(SexEncounterPatch));

        SceneManager.sceneLoaded += (UnityAction<Scene, LoadSceneMode>)OnSceneLoaded;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Log.Info($"Scene loaded: Name: {scene.name}, BuildIndex: {scene.buildIndex}");
        if (scene.buildIndex >= 2) {
            SexChoiceRealismMod.Prepare();
        }
    }
}
*/