using BaseMod.Core.Logger;

using Il2Cpp;

using MelonLoader;

using YC.GameTrainerMod;
using YC.GameTrainerMod.Components;
using YC.GameTrainerMod.Patches;

[assembly: MelonInfo(typeof(Plugin), MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION, MyPluginInfo.PLUGIN_AUTHORS)]
[assembly: MelonGame(null, null)]

namespace YC.GameTrainerMod;
public class Plugin : MelonMod
{
    internal static IPluginLogger Log;
    //internal static PluginConfig PluginConfig;
    //internal static string ConfigPath;

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();

        // Plugin startup logic
        Log = new MelonPluginLogger(new MelonLogger.Instance(MyPluginInfo.PLUGIN_GUID));
        /*ConfigPath  = MelonEnvironment.UserDataDirectory;
        PluginConfig = new($"{MyPluginInfo.PLUGIN_GUID}.cfg");*/

        HarmonyInstance.PatchAll(typeof(CombatTalentInventoryPatch));

        Log.Info($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        Log.Info($"Scene loaded: Name: {sceneName}, BuildIndex: {buildIndex}");
        if (buildIndex >= 2)
        {
            GameTrainerComponent.RegisterClass(Zessentials.Instance);
        }

        base.OnSceneWasLoaded(buildIndex, sceneName);
    }
}
