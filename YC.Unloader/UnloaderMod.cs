using MelonLoader;
using MelonLoader.Utils;
using YC.Unloader;
using YC.Unloader.Services;

[assembly: MelonInfo(typeof(UnloaderMod), ModInfo.NAME, ModInfo.VERSION, ModInfo.AUTHORS, ModInfo.URL)]
[assembly: MelonGame("Skyflare Studios", "Yaradiels Crown")]

namespace YC.Unloader;
public class UnloaderMod : MelonMod
{
    internal static MelonLogger.Instance Log;
    internal static string PluginResources;

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();

        // UnloaderMod startup logic
        Log = LoggerInstance;

        PluginResources = Path.Combine(MelonEnvironment.UserDataDirectory, ModInfo.GUID, "Resources");

        Log.Msg($"Mod {ModInfo.GUID} is loaded!");
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
}
