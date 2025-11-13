using System.IO;

using BepInEx;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;

using YC.EnemyRandomizerMod.Configs;
using YC.EnemyRandomizerMod.Mods;
using YC.EnemyRandomizerMod.Patches;

namespace YC.EnemyRandomizerMod.BepInEx;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin {
    internal static Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load() {
        // Plugin startup logic
        var logger = new BepInExLogger(Log);
        var config = ModConfig.Load(Path.Combine( Paths.ConfigPath, "YC.EnemyRandomizerMod.cfg") );

        EnemyBodyRandomizerMod.Load(logger, config);

        CombatEnemyManagerPatch.Init(logger);

        Harmony.PatchAll(typeof(CombatEnemyManagerPatch));

        logger.Info($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

}
