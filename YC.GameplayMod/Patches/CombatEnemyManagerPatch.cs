using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

using BaseMod.Core.Extensions;

using HarmonyLib;

using Il2Cpp;

using UnityEngine;

using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;
public class CombatEnemyManagerPatch {
    internal static bool Prepare() {
        try {
            if (!CharacterBodyRandomizerMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.Warn($"{nameof(CombatEnemyManagerPatch)} not applied due exeption");
            return false;
        }
    }
    private static bool _requested = false;

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CombatEnemyManager), nameof(CombatEnemyManager.RequestCharacterByName))]
    static bool CombatEnemyManagerRequestCharacterByNamePrefix(CombatEnemyManager __instance, bool __runOriginal, string characterName) {
        Plugin.Log.Info($"Request character: {characterName}");
        if (!string.IsNullOrWhiteSpace(characterName))
            _requested = true;

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CombatEnemyManager), nameof(CombatEnemyManager.RequestCharacterByName))]
    static void CombatEnemyManagerRequestCharacterByNamePostfix(CombatEnemyManager __instance, GameObject __result, string characterName) {
        if (__result is null)
            return;

        if (__result.TryGetComponentWithCast(out CharacterSex characterSex) && !string.IsNullOrWhiteSpace(characterSex.characterName)) {
            Plugin.Log.Info($"Customize character: {characterSex.characterName}");
            CharacterBodyRandomizerMod.Randomize(__instance, characterSex);
            characterSex.NPCSetup(characterSex.IsMale, characterSex.characterName, characterSex.characterAttributes.combatAI.isAlly);
            CharacterBodyRandomizerMod.SetFutaState(characterSex);
        }

        _requested = false;
        Plugin.Log.Info($"Request character: {characterName}");
    }

    /*[HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe), nameof(Wardrobe.Start))]
    static void WardrobeStartPostfix(Wardrobe __instance) {
        if (_requested)
            Plugin.Log.Info($"Start wardrobe character: {__instance.characterSex.characterName}");
    }*/

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe), nameof(Wardrobe.LoadStuff))]
    static void WardrobeLoadStuffPostfix(Wardrobe __instance) {
        if (_requested)
            Plugin.Log.Info($"Load wardrobe character: {__instance.characterSex.characterName}");
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterSex), nameof(CharacterSex.Start))]
    static void CharacterSexStartPostfix(CharacterSex __instance) {
        if (_requested)
            Plugin.Log.Info($"Start CharacterSex character: {__instance.characterName}");
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CombatEnemyManager), nameof(CombatEnemyManager.RandomizeEnemy))]
    static bool CombatEnemyManagerRandomizeEnemyPrefix(CombatEnemyManager __instance, bool __runOriginal) {
        //_requested = true;
        Plugin.Log.Debug($"randomize enemy");

        if (!__runOriginal)
            return false;

        return true;
    }

    /*[HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CombatEnemyManager), nameof(CombatEnemyManager.RandomizeEnemy))]
    static void CombatEnemyManagerRandomizeEnemyPostfix(CombatEnemyManager __instance) {
        //_requested = false;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Transform), "localScale", MethodType.Setter)]
    static bool Transformset_localScale_InjectedPrefix(Transform __instance, ref Vector3 value) {
        if (__instance.name.Equals("chest_size_R")) {
            var stack = new StackTrace(1);
            var caller = stack.GetFrame(0).GetMethod();
            Plugin.Log.Debug($"{__instance.name} in {caller.DeclaringType?.Name}.{caller.Name} set {value} ({stack})");
        }

        return true;
    }*/
}
