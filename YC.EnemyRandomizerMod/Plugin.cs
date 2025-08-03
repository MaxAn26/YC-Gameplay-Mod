using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;

using YC.EnemyRandomizerMod.Mods;
using YC.EnemyRandomizerMod.Plugins;

namespace YC.EnemyRandomizerMod;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin {
    internal static new ManualLogSource Log;
    internal static Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load() {
        // Plugin startup logic
        Log = base.Log;

        EnemyBodyRandomizerMod.Load(Config);

        Harmony.PatchAll(typeof(EnemyWardrobePatch));

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
