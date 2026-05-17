using BaseMod.Core.Extensions;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;

[HarmonyPatch(typeof(CombatEnemyManager))]
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
            Core.LogWarning($"{nameof(CombatEnemyManagerPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(CombatEnemyManager.RequestCharacterByName))]
    static bool CombatEnemyManagerRequestCharacterByNamePrefix(bool __runOriginal, string characterName)
    {
        Core.LogInfo($"Request character: {characterName}");

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(CombatEnemyManager.RequestCharacterByName))]
    static void CombatEnemyManagerRequestCharacterByNamePostfix(CombatEnemyManager __instance, GameObject __result)
    {
        if (__result is null)
        {
            return;
        }

        if (__result.TryGetComponentWithCast(out CharacterSex characterSex) && !string.IsNullOrWhiteSpace(characterSex.characterName))
        {
            Core.LogDebug($"Customize character: {characterSex.characterName}");
            CharacterBodyRandomizerMod.Randomize(__instance, characterSex);
            characterSex.NPCSetup(characterSex.IsMale, characterSex.characterName, characterSex.characterAttributes.combatAI.isAlly);
            CharacterBodyRandomizerMod.SetFutaState(characterSex);
        }
    }
}
