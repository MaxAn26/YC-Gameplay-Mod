using System.IO;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;

using UnityEngine.Events;
using UnityEngine.SceneManagement;

using YC.Unloader.Services;

namespace YC.Unloader;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin {
    internal static new ManualLogSource Log;
    internal static string PluginResources;
    internal static Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load() {
        // Plugin startup logic
        Log = base.Log;
        PluginResources = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");

        SceneManager.sceneLoaded += (UnityAction<Scene, LoadSceneMode>)OnSceneLoaded;

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == 2) {
            UnloadService.UnloadClothes();
            UnloadService.UnloadCombatBuffs();
            UnloadService.UnloadCombatTalents();
        }
    }
}
