using System;

using HarmonyLib;

using Il2Cpp;

using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;
internal class BattleManagerPatch {
    internal static bool Prepare() {
        try {
            if (!SexChoiceRealismMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.Warn($"{nameof(BattleManagerPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.ChangeSexPosition))]
    static void BattleManagerChangeSexPositionPrefix(CharacterAttributes __0, int __1) {
        Plugin.Log.Info($"Change position: Player attacker: {(__0.isPlayer ? "YES" : "no" )}, Sex Type: {__1}");
}

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.GetNewSexPosition))]
    static void BattleManagerGetNewSexPositionPostfix(CombatAction __0, ref int __result) {
        int newSexId = SexChoiceRealismMod.GetSexId(__0.caster.characterSex, __0.target.characterSex);
        if (newSexId > 0)
            __result = newSexId;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.GetNewSexPositionCharmed))]
    static void BattleManagerGetNewSexPositionCharmedPostfix(CombatAction __0, ref int __result) {
        int newSexId = SexChoiceRealismMod.GetSexId(__0.caster.characterSex, __0.target.characterSex);
        if (newSexId > 0)
            __result = newSexId;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.GetNewSexPositionSpanking))]
    static void BattleManagerGetNewSexPositionSpankingPostfix(CombatAction __0, ref int __result) {
        int newSexId = SexChoiceRealismMod.GetSexId(__0.caster.characterSex, __0.target.characterSex);
        if (newSexId > 0)
            __result = newSexId;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.AssistAlly))]
    static void BattleManagerAssistAllyPostfix(CombatAction __0, int __1) {
        Plugin.Log.Info($"Assist Ally {__0.actionName} '{__0.caster.characterSex.characterName}' -> '{__0.target.characterSex.characterName}': Slot: {__1}");
    }
}