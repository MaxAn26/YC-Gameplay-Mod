using System;
using System.Collections.Generic;
using System.Text;

using HarmonyLib;

using Il2Cpp;

using UnityEngine;

namespace YC.GameplayMod.Patches;
internal class DebugPatch {
    internal static bool Prepare() {
        try {


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
        message = $"[{System.DateTime.Now:HH:mm:ss}] {message.ToString()}";

        if (!__runOriginal)
            return false;

        return true;
    }
#endif
}