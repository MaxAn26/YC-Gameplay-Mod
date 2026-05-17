using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using YC.GameplayMod.Models;
using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;

[HarmonyPatch(typeof(BattleManager))]
internal class BattleManagerPatch
{
    [HarmonyPrepare]
    internal static bool Prepare()
    {
        try
        {
            if (!SexChoiceRealismMod.IsModActive)
            {
                return false;
            }

            return true;
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"{nameof(BattleManagerPatch)} not applied due exeption {ex.Message}");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(BattleManager.GetNewSexPosition))]
    static void BattleManagerGetNewSexPositionPostfix(CombatAction __0, ref int __result)
    {
        int newSexId = SexChoiceRealismMod.GetSexId(__0.caster.characterSex, __0.target.characterSex);
        if (newSexId > 0)
        {
            __result = newSexId;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(BattleManager.GetNewSexPositionCharmed))]
    static void BattleManagerGetNewSexPositionCharmedPostfix(CombatAction __0, ref int __result)
    {
        Core.LogInfo("Request Charmed position");
        int newSexId = SexChoiceRealismMod.GetSexId(__0.caster.characterSex, __0.target.characterSex, PositionActionMode.Command);
        if (newSexId > 0)
        {
            __result = newSexId;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(BattleManager.GetNewSexPositionSpanking))]
    static void BattleManagerGetNewSexPositionSpankingPostfix(CombatAction __0, ref int __result)
    {
        Core.LogInfo("Request Spanking position");
        int newSexId = SexChoiceRealismMod.GetSexId(__0.caster.characterSex, __0.target.characterSex, SexTag.Spanking);
        if (newSexId > 0)
        {
            __result = newSexId;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(BattleManager.AssistAlly))]
    static bool BattleManagerAssistAllyPrefix(bool __runOriginal, ref CombatAction action)
    {
        if (action is not null)
        {
            GameExtendMod.BattleManagerAssistAlly(ref action);
        }

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(BattleManager.ExecuteAction))]
    static bool BattleManagerExecuteActionPrefix(bool __runOriginal, CombatAction action)
    {
        if (action.actionType is 1 or 2)
        {
            GameExtendMod.BattleManagerExecuteAction(ref action);
        }

        Core.LogInfo($"Execute Action: {action.actionName}, Caster: {action.caster.characterName}, Target: {action.target?.characterName}");

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }
}
