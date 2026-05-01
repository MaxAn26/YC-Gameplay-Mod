using BaseMod.Core.Extensions;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;
public class CombatEnemyManagerPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!CharacterBodyRandomizerMod.IsModActive)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            GameplayMod.Log.Warning($"{nameof(CombatEnemyManagerPatch)} not applied due exeption");
            return false;
        }
    }
    private static bool _requested = false;

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CombatEnemyManager), nameof(CombatEnemyManager.RequestCharacterByName))]
    static bool CombatEnemyManagerRequestCharacterByNamePrefix(bool __runOriginal, string characterName)
    {
        GameplayMod.Log.Msg($"Request character: {characterName}");
        if (!string.IsNullOrWhiteSpace(characterName))
        {
            _requested = true;
        }

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CombatEnemyManager), nameof(CombatEnemyManager.RequestCharacterByName))]
    static void CombatEnemyManagerRequestCharacterByNamePostfix(CombatEnemyManager __instance, GameObject __result, string characterName)
    {
        if (__result is null)
        {
            return;
        }

        if (__result.TryGetComponentWithCast(out CharacterSex characterSex) && !string.IsNullOrWhiteSpace(characterSex.characterName))
        {
            GameplayMod.Log.Msg($"Customize character: {characterSex.characterName}");
            CharacterBodyRandomizerMod.Randomize(__instance, characterSex);
            characterSex.NPCSetup(characterSex.IsMale, characterSex.characterName, characterSex.characterAttributes.combatAI.isAlly);
            CharacterBodyRandomizerMod.SetFutaState(characterSex);
        }

        _requested = false;
        GameplayMod.Log.Msg($"Request character: {characterName}");
    }

    /*[HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe), nameof(Wardrobe.Start))]
    static void WardrobeStartPostfix(Wardrobe __instance) {
        if (_requested)
            GameplayMod.Log.Msg($"Start wardrobe character: {__instance.characterSex.characterName}");
    }*/

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe), nameof(Wardrobe.LoadStuff))]
    static void WardrobeLoadStuffPostfix(Wardrobe __instance)
    {
        if (_requested)
        {
            GameplayMod.Log.Msg($"Load wardrobe character: {__instance.characterSex.characterName}");
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterSex), nameof(CharacterSex.Start))]
    static void CharacterSexStartPostfix(CharacterSex __instance)
    {
        if (_requested)
        {
            GameplayMod.Log.Msg($"Start CharacterSex character: {__instance.characterName}");
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CombatEnemyManager), nameof(CombatEnemyManager.RandomizeEnemy))]
    static bool CombatEnemyManagerRandomizeEnemyPrefix(bool __runOriginal)
    {
        //_requested = true;
        GameplayMod.Log.Msg($"randomize enemy");

        if (!__runOriginal)
        {
            return false;
        }

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
            GameplayMod.Log.Msg($"{__instance.name} in {caller.DeclaringType?.Name}.{caller.Name} set {value} ({stack})");
        }

        return true;
    }*/
}
