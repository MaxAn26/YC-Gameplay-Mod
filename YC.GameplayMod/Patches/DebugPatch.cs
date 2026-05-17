using HarmonyLib;
using MelonLoader.Utils;
using UnityEngine;

namespace YC.GameplayMod.Patches;

#if DEBUG

[HarmonyPatch(typeof(Debug))]
internal class DebugPatch
{
    private static string _logFile = "GameLog.txt";

    internal static bool Prepare()
    {
        try
        {
            _logFile = $"GameLog-{DateTime.Now:dd_MM_yy-HH_mm_ss}.txt";

            return true;
        }
        catch (Exception)
        {
            Core.LogWarning($"{nameof(DebugPatch)} not applied due exeption");
            return false;
        }
    }


    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(Debug.Log), [typeof(Il2CppSystem.Object)])]
    [HarmonyPatch(nameof(Debug.LogWarning), [typeof(Il2CppSystem.Object)])]
    [HarmonyPatch(nameof(Debug.LogError), [typeof(Il2CppSystem.Object)])]
    [HarmonyPatch(nameof(Debug.LogAssertion), [typeof(Il2CppSystem.Object)])]
    static bool DebugLogPrefix(bool __runOriginal, ref Il2CppSystem.Object message)
    {
        string log = Path.Combine(MelonEnvironment.MelonLoaderLogsDirectory, _logFile);

        File.AppendAllText(log, $"[{System.DateTime.Now:HH:mm:ss}] {message.ToString()}\r\n");

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }
}
#endif
