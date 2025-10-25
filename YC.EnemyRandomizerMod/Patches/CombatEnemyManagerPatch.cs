using System;

using BaseMod.Core.Extensions;

using HarmonyLib;

using UnityEngine;

using YC.EnemyRandomizerMod.Mods;

namespace YC.EnemyRandomizerMod.Patches;
internal class CombatEnemyManagerPatch {
    internal static bool Prepare() {
        try {
            if (!EnemyBodyRandomizerMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.LogWarning($"{nameof(CombatEnemyManagerPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CombatEnemyManager), nameof(CombatEnemyManager.RequestCharacterByName))]
    static void CombatEnemyManagerRequestCharacterByNamePostfix(CombatEnemyManager __instance, GameObject __result ) {
        if (__result is null)
            return;
        
        if (__result.TryGetComponentWithCast(out CharacterSex characterSex) && !string.IsNullOrWhiteSpace(characterSex.characterName) ) {
            Plugin.Log.Info($"Customize character: {characterSex.characterName}");
            EnemyBodyRandomizerMod.Apply(__instance, characterSex, characterSex.wardrobe);
            characterSex.NPCSetup(characterSex.IsMale, characterSex.characterName, characterSex.characterAttributes.combatAI.isAlly);
            EnemyBodyRandomizerMod.SetFutaState(characterSex, characterSex.wardrobe);
        }
    }
}