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

    /*[HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe2), nameof(Wardrobe2.CompleteClothingSetup), [typeof(LoadContext), typeof(GameObject)])]
    static bool Wardrobe2CompleteClothingSetupPrefix(Wardrobe2 __instance, bool __runOriginal, LoadContext SetContext, GameObject TargetObjt) {
        Plugin.Log.Debug($"Prefix CompleteClothingSetup, context: {SetContext.Sex.characterName}, target object: {TargetObjt.name}");

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe2), nameof(Wardrobe2.CompleteClothingSetup), [typeof(LoadContext), typeof(GameObject)])]
    static void Wardrobe2CompleteClothingSetupPostfix(Wardrobe2 __instance, LoadContext SetContext, GameObject TargetObjt) {
        Plugin.Log.Debug($"Postfix CompleteClothingSetup, context: {SetContext.Sex.characterName}, target object: {TargetObjt.name}");
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe2), nameof(Wardrobe2.CompleteClothingSetup), [typeof(CharacterSex), typeof(LoadContext), typeof(GameObject)])]
    static bool Wardrobe2CompleteClothingSetup2Prefix(Wardrobe2 __instance, bool __runOriginal, CharacterSex PlayerSex, LoadContext SetContext, GameObject TargetObjt) {
        Plugin.Log.Debug($"Prefix CompleteClothingSetup, player: {PlayerSex.characterName} context: {SetContext.Sex.characterName}, target object: {TargetObjt.name}");

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe2), nameof(Wardrobe2.CompleteClothingSetup), [typeof(CharacterSex), typeof(LoadContext), typeof(GameObject)])]
    static void Wardrobe2CompleteClothingSetup2Postfix(Wardrobe2 __instance, CharacterSex PlayerSex, LoadContext SetContext, GameObject TargetObjt) {
        Plugin.Log.Debug($"Postfix CompleteClothingSetup, player: {PlayerSex.characterName} context: {SetContext.Sex.characterName}, target object: {TargetObjt.name}");
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe2), nameof(Wardrobe2.LoadStuff), [typeof(bool)])]
    static bool Wardrobe2LoadStuffPrefix(Wardrobe2 __instance, bool __runOriginal, bool isEnemyChar) {
        Plugin.Log.Debug($"Prefix load, isEnemy: {(isEnemyChar ? "Yes" : "No")}");

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe2), nameof(Wardrobe2.LoadStuff), [typeof(bool)])]
    static void Wardrobe2LoadStuffPostfix(Wardrobe2 __instance, bool isEnemyChar) {
        Plugin.Log.Debug($"Postfix load, isEnemy: {(isEnemyChar ? "Yes" : "No")}");
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe2), nameof(Wardrobe2.LoadStuff), [typeof(bool), typeof(GameObject), typeof(EnemyData)])]
    static bool Wardrobe2LoadStuff2Prefix(Wardrobe2 __instance, bool __runOriginal, bool isEnemyChar, GameObject UseObjt, EnemyData UseData) {
        Plugin.Log.Debug($"Prefix load, isEnemy: {(isEnemyChar ? "Yes" : "No")}, use object: {UseObjt.name}, enemy data: {UseData.characterName}");

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe2), nameof(Wardrobe2.LoadStuff), [typeof(bool), typeof(GameObject), typeof(EnemyData)])]
    static void Wardrobe2LoadStuff2Postfix(Wardrobe2 __instance, bool isEnemyChar, GameObject UseObjt, EnemyData UseData) {
        Plugin.Log.Debug($"Postfix load, isEnemy: {(isEnemyChar ? "Yes" : "No")}, use object: {UseObjt.name}, enemy data: {UseData.characterName}");
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe2), nameof(Wardrobe2.LoadClothingBySlot))]
    static bool Wardrobe2LoadClothingBySlotPrefix(Wardrobe2 __instance, bool __runOriginal, int slot, int Idder, GameObject TargetObjt, int materialIndex, Color materialColor, float materialSmoothness, LoadContext SetContext) {
        Plugin.Log.Debug($"Prefix LoadClothingBySlot, slot: {slot}, idDer: {Idder}, Target object: {TargetObjt.name}, Material index: {materialIndex}, Color: {materialColor}, Smoothness: {materialSmoothness}");

        if (!__runOriginal)
            return false;

        return true;
    }*/

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe2), nameof(Wardrobe2.LoadClothingBySlot))]
    static void Wardrobe2LoadClothingBySlotPostfix(Wardrobe2 __instance, int slot, int Idder, GameObject TargetObjt, int materialIndex, Color materialColor, float materialSmoothness, LoadContext SetContext) {
        Plugin.Log.Debug($"Postfix LoadClothingBySlot, slot: {slot}, idDer: {Idder}, Target object: {TargetObjt.name}, Material index: {materialIndex}, Color: {materialColor}, Smoothness: {materialSmoothness}");
    }
#endif
}
