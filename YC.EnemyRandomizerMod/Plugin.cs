using BaseMod.Core;
using BaseMod.Core.Logger;

using MelonLoader;
using MelonLoader.Utils;

using YC.EnemyRandomizerMod;
using YC.EnemyRandomizerMod.Mods;

using YC.EnemyRandomizerMod.Patches;

[assembly: MelonInfo(typeof(Plugin), MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION, MyPluginInfo.PLUGIN_AUTHORS)]
[assembly: MelonGame(null, null)]

namespace YC.EnemyRandomizerMod;
public class Plugin : MelonMod {
    internal static IPluginLogger Log;
    internal static string ConfigPath;

    public override void OnInitializeMelon() {
        base.OnInitializeMelon();

        // Plugin startup logic
        Log         = new MelonPluginLogger( new MelonLogger.Instance(MyPluginInfo.PLUGIN_GUID));
        ConfigPath  = MelonEnvironment.UserDataDirectory;

        PluginConfig config = new($"{MyPluginInfo.PLUGIN_GUID}.cfg");
        
        EnemyBodyRandomizerMod.Load(config);

        HarmonyInstance.PatchAll(typeof(CombatEnemyManagerPatch));

        Log.Info($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
