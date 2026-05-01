using BaseMod.Core;
using MelonLoader;
using MelonLoader.Utils;
using YC.GameplayMod;
using YC.GameplayMod.Mods;
using YC.GameplayMod.Patches;

[assembly: MelonInfo(typeof(GameplayMod), ModInfo.MOD_NAME, ModInfo.MOD_VERSION, ModInfo.MOD_DEVELOPER, ModInfo.MOD_URL)]
[assembly: MelonGame(ModInfo.GAME_DEVELOPER, ModInfo.GAME_NAME)]

namespace YC.GameplayMod;
public class GameplayMod : MelonMod
{
    internal static MelonLogger.Instance Log;
    internal static PluginConfig PluginConfig;
    internal static string ConfigPath;
    internal static string PluginAssets;
    internal static string PluginConfigs;
    internal static string PluginResources;

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();

        // GameplayMod startup logic
        Log = LoggerInstance;
        ConfigPath = MelonEnvironment.UserDataDirectory;
        PluginAssets = Path.Combine(ConfigPath, ModInfo.MOD_GUID, "Assets");
        PluginConfigs = Path.Combine(ConfigPath, ModInfo.MOD_GUID, "Configs");
        PluginResources = Path.Combine(ConfigPath, ModInfo.MOD_GUID, "Resources");

        PluginConfig = new($"{ModInfo.MOD_GUID}.cfg");

        DickStraponVisibilityMod.Load(PluginConfig);
        CharacterBodyRandomizerMod.Load(PluginConfig);
        GameExtendMod.Load(PluginConfig);
        GameFixMod.Load(PluginConfig);
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

        Log.Msg($"Mod {ModInfo.MOD_GUID} is loaded!");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        Log.Msg($"Scene loaded: Name: {sceneName}, BuildIndex: {buildIndex}");
        if (buildIndex >= 2)
        {
            SexChoiceRealismMod.Prepare();
        }

        base.OnSceneWasLoaded(buildIndex, sceneName);
    }

    public override void OnPreferencesSaved()
    {
        PluginConfig?.Save();

        base.OnPreferencesSaved();
    }
}
