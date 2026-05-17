using System.Runtime.CompilerServices;
using MelonLoader;
using MelonLoader.Logging;
using MelonLoader.Utils;
using YC.Unloader;
using YC.Unloader.Services;

[assembly: MelonInfo(typeof(Core), ModInfo.MOD_NAME, ModInfo.MOD_VERSION, ModInfo.MOD_DEVELOPER, ModInfo.MOD_URL)]
[assembly: MelonGame(ModInfo.GAME_DEVELOPER, ModInfo.GAME_NAME)]

namespace YC.Unloader;
public class Core : MelonMod
{
    internal static string PluginResources;

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();

        // Core startup logic
        PluginResources = Path.Combine(MelonEnvironment.UserDataDirectory, ModInfo.MOD_GUID, "Resources");

        LogInfo($"Mod {ModInfo.MOD_GUID} is loaded!");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (buildIndex is 2)
        {
            UnloadService.UnloadClothes();
            UnloadService.UnloadColors();
            UnloadService.UnloadCombatActions();
            UnloadService.UnloadCombatBuffs();
            UnloadService.UnloadCombatTalents();
            UnloadService.UnloadCombatEnemyPassives();
            UnloadService.UnloadInventoryItems();
        }

        base.OnSceneWasLoaded(buildIndex, sceneName);
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
