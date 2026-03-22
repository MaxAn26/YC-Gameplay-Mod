using System.IO;

using BaseMod.Core;
using BaseMod.Core.Logger;

using MelonLoader;
using MelonLoader.Utils;

using YC.GameplayMod;
using YC.GameplayMod.Mods;
using YC.GameplayMod.Patches;

[assembly: MelonInfo(typeof(Plugin), MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION, MyPluginInfo.PLUGIN_AUTHORS)]
[assembly: MelonGame(null, null)]

namespace YC.GameplayMod;
public class Plugin : MelonMod {
    internal static IPluginLogger Log;
    internal static PluginConfig PluginConfig;
    internal static string ConfigPath;
    internal static string PluginAssets;
    internal static string PluginConfigs;
    internal static string PluginResources;

    public override void OnInitializeMelon() {
        base.OnInitializeMelon();

        // Plugin startup logic
        Log             = new MelonPluginLogger( new MelonLogger.Instance(MyPluginInfo.PLUGIN_GUID));
        ConfigPath      = MelonEnvironment.UserDataDirectory;
        PluginAssets    = Path.Combine(ConfigPath, MyPluginInfo.PLUGIN_GUID, "Assets");
        PluginConfigs   = Path.Combine(ConfigPath, MyPluginInfo.PLUGIN_GUID, "Configs");
        PluginResources = Path.Combine(ConfigPath, MyPluginInfo.PLUGIN_GUID, "Resources");

        PluginConfig = new($"{MyPluginInfo.PLUGIN_GUID}.cfg");

        DickStraponVisibilityMod.Load(PluginConfig);
        CharacterBodyRandomizerMod.Load(PluginConfig);
        GameExtendMod.Load(PluginConfig);
        RandomReverseMod.Load(PluginConfig);
        SexChoiceRealismMod.Load(PluginConfig);

        HarmonyInstance.PatchAll(typeof(BattleManagerPatch));
        HarmonyInstance.PatchAll(typeof(CharacterAttributesPatch));
        HarmonyInstance.PatchAll(typeof(CharacterSexPatch));
        HarmonyInstance.PatchAll(typeof(CombatEnemyManagerPatch));
        HarmonyInstance.PatchAll(typeof(SexEncounterPatch));

#if DEBUG
        HarmonyInstance.PatchAll(typeof(DebugPatch));
#endif

        Log.Info($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
        Log.Info($"Scene loaded: Name: {sceneName}, BuildIndex: {buildIndex}");
        if (buildIndex >= 2) {
            SexChoiceRealismMod.Prepare();
        }

        base.OnSceneWasLoaded(buildIndex, sceneName);
    }

    public override void OnPreferencesSaved() {
        PluginConfig?.Save();

        base.OnPreferencesSaved();
    }
}
