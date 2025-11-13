using System.IO;

using BaseMod.Core.Logger;

using MelonLoader;
using MelonLoader.Utils;

using YC.EnemyRandomizerMod;
using YC.EnemyRandomizerMod.Configs;
using YC.EnemyRandomizerMod.Mods;

using YC.EnemyRandomizerMod.Patches;

[assembly: MelonInfo(typeof(Plugin), MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION, MyPluginInfo.PLUGIN_AUTHORS)]
[assembly: MelonGame(null, null)]

namespace YC.EnemyRandomizerMod;
public class Plugin : MelonMod {
    internal static IPluginLogger Log;

    public override void OnInitializeMelon() {
        base.OnInitializeMelon();

        // Plugin startup logic
        Log = new MelonPluginLogger( new MelonLogger.Instance(MyPluginInfo.PLUGIN_GUID));
        
        var config = ModConfig.Load(Path.Combine( MelonEnvironment.UserDataDirectory, $"{MyPluginInfo.PLUGIN_GUID}.cfg") );

        EnemyBodyRandomizerMod.Load(config);

        HarmonyInstance.PatchAll(typeof(CombatEnemyManagerPatch));

        Log.Info($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
