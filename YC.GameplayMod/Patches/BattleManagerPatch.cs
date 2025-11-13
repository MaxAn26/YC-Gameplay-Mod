using System;

using BaseMod.Core.Extensions;

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
    static void BattleManagerGetNewSexPositionPostfix(CombatAction __0) {
        Plugin.Log.Info($"Get new position: Combat action: {__0.actionName}");
    }
}