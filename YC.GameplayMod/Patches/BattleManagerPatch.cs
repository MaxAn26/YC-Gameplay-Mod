using HarmonyLib;

using Il2Cpp;

using UnityEngine;

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

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.AssistAlly))]
    static void BattleManagerAssistAllyPrefix(ref CombatAction __0, int __1) {
        if (__0 is not null) {
            GameFixMod.AssistAllyFix(ref __0);
        }

        Plugin.Log.Info($"Assist Ally {__0.actionName} '{__0.caster.characterSex.characterName}' -> '{__0.target.characterSex.characterName}': Slot: {__1}");
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(BattleManager), nameof(BattleManager.PrepareAction))]
    static bool BattleManagerPrepareActionPrefix(BattleManager __instance, bool __runOriginal, ref CombatAction __0) {
        Plugin.Log.Debug($"Action: {__0.actionName}");

        if (!__runOriginal)
            return false;

        return true;
    }
}
