using System.Runtime.CompilerServices;
using BaseMod.Core;
using MelonLoader;
using MelonLoader.Logging;
using MelonLoader.Utils;
using YC.GameplayMod;
using YC.GameplayMod.Mods;

[assembly: HarmonyDontPatchAll]
[assembly: MelonInfo(typeof(Core), ModInfo.MOD_NAME, ModInfo.MOD_VERSION, ModInfo.MOD_DEVELOPER, ModInfo.MOD_URL)]
[assembly: MelonGame(ModInfo.GAME_DEVELOPER, ModInfo.GAME_NAME)]

namespace YC.GameplayMod;
public class Core : MelonMod
{
    internal static PluginConfig PluginConfig;
    internal static string ConfigPath;
    internal static string PluginAssets;
    internal static string PluginConfigs;
    internal static string PluginResources;

    public override void OnInitializeMelon()
    {
        // Core startup logic
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

        HarmonyInstance.PatchAll();

        base.OnInitializeMelon();

        LogInfo($"Mod {ModInfo.MOD_GUID} is loaded!");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        LogInfo($"Scene loaded: Name: {sceneName}, BuildIndex: {buildIndex}");
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

    public static void LogTrace(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.DarkGray, Combine(message, methodName));
    public static void LogDebug(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.Cyan, Combine(message, methodName));
    public static void LogInfo(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.White, Combine(message, methodName));
    public static void LogSuccess(string message, [CallerMemberName] string methodName = null) => MelonLogger.Msg(ColorARGB.Green, Combine(message, methodName));
    public static void LogWarning(string message, [CallerMemberName] string methodName = null) => MelonLogger.Warning(Combine(message, methodName));
    public static void LogError(Exception excepion, [CallerMemberName] string methodName = null) => LogError(excepion.Message, excepion, methodName);
    public static void LogError(string message, Exception excepion, [CallerMemberName] string methodName = null) => MelonLogger.Error(Combine(message, methodName), excepion);

    private static string Combine(string message, string methodName) => !string.IsNullOrWhiteSpace(methodName) ? $"{methodName}:> {message}" : message;
}
