using System.IO;

using MelonLoader;
using MelonLoader.Utils;

using YC.EnemyRandomizerMod.Configs;
using YC.EnemyRandomizerMod.Melon;
using YC.EnemyRandomizerMod.Mods;

using YC.EnemyRandomizerMod.Patches;

[assembly: MelonInfo(typeof(Plugin), "YC.EnemyRandomizerMod", "1.0", "BoL4oNoK")]
[assembly: MelonGame(null, null)]

namespace YC.EnemyRandomizerMod.Melon;
public class Plugin : MelonMod {
    public override void OnInitializeMelon() {
        base.OnInitializeMelon();

        // Plugin startup logic
        var logger = new MelonPluginLogger( new MelonLogger.Instance("YC.EnemyRandomizerMod"));
        
        var config = ModConfig.Load(Path.Combine( MelonEnvironment.UserDataDirectory, "YC.EnemyRandomizerMod.cfg") );

        EnemyBodyRandomizerMod.Load(logger, config);

        CombatEnemyManagerPatch.Init(logger);

        HarmonyInstance.PatchAll(typeof(CombatEnemyManagerPatch));

        logger.Info($"Plugin YC.EnemyRandomizerMod is loaded!");
    }
}
