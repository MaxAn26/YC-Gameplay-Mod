using HarmonyLib;
using Il2Cpp;
using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;

[HarmonyPatch(typeof(SexEncounter))]
internal class SexEncounterPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!SexChoiceRealismMod.IsModActive && !DickStraponVisibilityMod.IsModActive && !GameExtendMod.IsModActive)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            Core.LogWarning($"{nameof(SexEncounterPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexEncounter.SetDicks))]
    static bool SexEncounterSetDicksPrefix(ref SexEncounter __instance, bool __runOriginal)
    {
        GameFixMod.SexEncounerSetSexAnimation(ref __instance);
        bool result = DickStraponVisibilityMod.SetDicks(__instance);

        if (!__runOriginal)
        {
            return false;
        }

        return !result;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexEncounter.CounterAction))]
    static bool SexEncounterCounterActionPrefix(SexEncounter __instance, bool __runOriginal, ref bool CasterChanged, ref int newSexID)
    {
        int SexId = CasterChanged
            ? SexChoiceRealismMod.GetSexId(__instance.TargetSex, __instance.CasterSex, __instance.IsThreesome ? __instance.AssistSex : null)
            : SexChoiceRealismMod.GetSexId(__instance.CasterSex, __instance.TargetSex, __instance.IsThreesome ? __instance.AssistSex : null);

        if (SexId > 0)
        {
            newSexID = SexId;
        }

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexEncounter.JoinThreesome))]
    static bool SexEncounterJoinThreesomePrefix(SexEncounter __instance, bool __runOriginal, ref CharacterAttributes character, ref int newSexID)
    {
        if (!__runOriginal)
        {
            return false;
        }

        if (GameExtendMod.SexEncounterJoinThreesome(__instance, character, newSexID))
        {
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexEncounter.SetSexAnimation))]
    static void SexEncounterSetSexAnimationPostfix(SexEncounter __instance) => RandomReverseMod.Apply(__instance);

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(SexEncounter.SetThreesomeAnimation))]
    static void SexEncounterSetThreesomeAnimationPostfix(SexEncounter __instance) => RandomReverseMod.Apply(__instance);
}
