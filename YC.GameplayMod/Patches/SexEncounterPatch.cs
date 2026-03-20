using System;

using HarmonyLib;

using Il2Cpp;

using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;
internal class SexEncounterPatch {
    internal static bool Prepare() {
        try {
            if (!SexChoiceRealismMod.IsModActive && !DickStraponVisibilityMod.IsModActive && !GameFixMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.Warn($"{nameof(SexEncounterPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexEncounter), nameof(SexEncounter.SetDicks))]
    static bool SexEncounterSetDicksPrefix(SexEncounter __instance, bool __runOriginal) {
        bool result = DickStraponVisibilityMod.SetDicks(__instance);

        if (!__runOriginal)
            return false;

        return !result;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexEncounter), nameof(SexEncounter.CounterAction))]
    static bool SexEncounterCounterActionPrefix(SexEncounter __instance, bool __runOriginal, ref bool __0, ref int __1) {
        int newSexId;
        if (__0)
            newSexId = SexChoiceRealismMod.GetSexId(__instance.TargetSex, __instance.CasterSex, __instance.IsThreesome ? __instance.AssistSex : null);
        else
            newSexId = SexChoiceRealismMod.GetSexId(__instance.CasterSex, __instance.TargetSex, __instance.IsThreesome ? __instance.AssistSex : null);

        if (newSexId > 0)
            __1 = newSexId;

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexEncounter), nameof(SexEncounter.JoinThreesome))]
    static bool SexEncounterJoinThreesomePrefix(SexEncounter __instance, bool __runOriginal, ref CharacterAttributes __0, ref int __1) {
        if (!__runOriginal)
            return false;

        if (GameFixMod.SexEncounterJoinThreesome(__instance, __0, __1))
            return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexEncounter), nameof(SexEncounter.SetSexAnimation))]
    static void SexEncounterSetSexAnimationPostfix(SexEncounter __instance) {
        RandomReverseMod.Apply(__instance);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexEncounter), nameof(SexEncounter.SetThreesomeAnimation))]
    static void SexEncounterSetThreesomeAnimationPostfix(SexEncounter __instance) {
        RandomReverseMod.Apply(__instance);
    }
}
