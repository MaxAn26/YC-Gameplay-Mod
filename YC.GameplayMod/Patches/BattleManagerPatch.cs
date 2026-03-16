using HarmonyLib;

using Il2Cpp;

using UnityEngine;

using YC.GameplayMod.Models;
using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;
internal class BattleManagerPatch {
    internal static bool Prepare() {
        try {
            if (!SexChoiceRealismMod.IsModActive)
                return false;

            return true;
        } catch (System.Exception) {
            Plugin.Log.Warn($"{nameof(BattleManagerPatch)} not applied due exeption");
            return false;
        }
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
        Plugin.Log.Debug("Request Charmed position");
        int newSexId = SexChoiceRealismMod.GetSexId(__0.caster.characterSex, __0.target.characterSex, PositionActionMode.Command);
        if (newSexId > 0)
            __result = newSexId;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.GetNewSexPositionSpanking))]
    static void BattleManagerGetNewSexPositionSpankingPostfix(CombatAction __0, ref int __result) {
        Plugin.Log.Debug("Request Spanking position");
        int newSexId = SexChoiceRealismMod.GetSexId(__0.caster.characterSex, __0.target.characterSex, SexTag.Spanking);
        if (newSexId > 0)
            __result = newSexId;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.AssistAlly))]
    static bool BattleManagerAssistAllyPrefix(bool __runOriginal, ref CombatAction __0) {
        if (__0 is not null) {
            GameFixMod.AssistAllyFix(ref __0);
        }

        if (!__runOriginal)
            return false;

        return true;
    }

    /*[HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.PrepareAction))]
    static bool BattleManagerPrepareActionPrefix(bool __runOriginal, ref CombatAction __0) {
        if (__0.actionType is 1 or 2)
            GameFixMod.AllyAttackTargetFix(ref __0);

        Plugin.Log.Debug($"Prepare Action: {__0.actionName}, Caster: {__0.caster.characterName}, Target: {__0.target?.characterName}");

        if (!__runOriginal)
            return false;

        return true;
    }*/

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.ExecuteAction))]
    static bool BattleManagerExecuteActionPrefix(bool __runOriginal, CombatAction __0) {
        if (__0.actionType is 1 or 2)
            GameFixMod.AllyAttackTargetFix(ref __0);

        Plugin.Log.Debug($"Execute Action: {__0.actionName}, Caster: {__0.caster.characterName}, Target: {__0.target?.characterName}");

        if (!__runOriginal)
            return false;

        return true;
    }
}
