using System.IO;

using BaseMod.Core.Logger;

using MelonLoader;
using MelonLoader.Utils;

using YC.Unloader;
using YC.Unloader.Services;

[assembly: MelonInfo(typeof(Plugin), MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION, MyPluginInfo.PLUGIN_AUTHORS)]
[assembly: MelonGame(null, null)]

namespace YC.Unloader;
public class Plugin : MelonMod {
    internal static IPluginLogger Log;
    internal static string PluginResources;

    public override void OnInitializeMelon() {
        base.OnInitializeMelon();

        // Plugin startup logic
        Log = new MelonPluginLogger(new MelonLogger.Instance(MyPluginInfo.PLUGIN_GUID));
        
        PluginResources = Path.Combine(MelonEnvironment.UserDataDirectory, MyPluginInfo.PLUGIN_GUID, "Resources");

        Log.Info($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
        if (buildIndex == 2) {
            UnloadService.UnloadClothes();
            UnloadService.UnloadCombatActions();
            UnloadService.UnloadCombatBuffs();
            UnloadService.UnloadCombatTalents();
        }

        base.OnSceneWasLoaded(buildIndex, sceneName);
    }
}
