using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using HarmonyLib;

using Il2Cpp;

using MelonLoader.Utils;

using UnityEngine;

namespace YC.GameplayMod.Patches;
internal class DebugPatch {
    private static string _logFile = "GameLog.txt";

    internal static bool Prepare() {
        try {
            _logFile = $"GameLog-{DateTime.Now:dd_MM_yy-HH_mm_ss}.txt";

            return true;
        } catch (Exception) {
            Plugin.Log.Warn($"{nameof(DebugPatch)} not applied due exeption");
            return false;
        }
    }

#if DEBUG
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.RemoveCharacterFromList))]
    static bool BattleManagerRemoveCharacterFromListPrefix(bool __runOriginal, CharacterAttributes __0) {
        BattleManager.print($"Remove character: {__0.characterName}");

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Debug), nameof(Debug.Log), [typeof(Il2CppSystem.Object)])]
    [HarmonyPatch(typeof(Debug), nameof(Debug.LogWarning), [typeof(Il2CppSystem.Object)])]
    [HarmonyPatch(typeof(Debug), nameof(Debug.LogError), [typeof(Il2CppSystem.Object)])]
    [HarmonyPatch(typeof(Debug), nameof(Debug.LogAssertion), [typeof(Il2CppSystem.Object)])]
    static bool DebugLogPrefix(bool __runOriginal, ref Il2CppSystem.Object message) {
        string log = Path.Combine(MelonEnvironment.MelonLoaderLogsDirectory, _logFile);

        File.AppendAllText( log, $"[{System.DateTime.Now:HH:mm:ss}] {message.ToString()}\r\n");

        if (!__runOriginal)
            return false;

        return true;
    }
#endif
}
