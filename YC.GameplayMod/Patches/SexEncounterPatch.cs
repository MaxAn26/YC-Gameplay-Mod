using System;

using BaseMod.Core.Extensions;

using HarmonyLib;

using Il2Cpp;

using Il2CppSystem.Diagnostics;

using UnityEngine;

using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;
internal class SexEncounterPatch {
    internal static bool Prepare() {
        try {
            if (!SexChoiceRealismMod.IsModActive && !DickStraponVisibilityMod.IsModActive)
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
    [HarmonyPatch(typeof(SexEncounter), nameof(SexEncounter.SetHeavyBondageAnimation))]
    static bool SexEncounterSetHeavyBondageAnimationPrefix(SexEncounter __instance, bool __runOriginal) {
        SexChoiceRealismMod.SetSexID(__instance);

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexEncounter), nameof(SexEncounter.SetSexAnimation))]
    static bool SexEncounterSetSexAnimationPrefix(SexEncounter __instance, bool __runOriginal) {
        SexChoiceRealismMod.SetSexID(__instance);

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexEncounter), nameof(SexEncounter.SetThreesomeAnimation))]
    static bool SexEncounterSetThreesomeAnimationPrefix(SexEncounter __instance, bool __runOriginal) {
        SexChoiceRealismMod.SetThreesomeSexID(__instance);

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexEncounter), nameof(SexEncounter.SetHeavyBondageAnimation))]
    static void SexEncounterSetHeavyBondageAnimationPostfix(SexEncounter __instance) {
        Plugin.Log.Info($"HeavyBondageAnimation: ID: {__instance.SexID}, Type: {__instance.SexType}");
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexEncounter), nameof(SexEncounter.SetSexAnimation))]
    static void SexEncounterSetSexAnimationPostfix(SexEncounter __instance) {
        Plugin.Log.Info($"SexAnimation: ID: {__instance.SexID}, Type: {__instance.SexType}");
        RandomReverseMod.Apply(__instance);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexEncounter), nameof(SexEncounter.SetThreesomeAnimation))]
    static void SexEncounterSetThreesomeAnimationPostfix(SexEncounter __instance) {
        Plugin.Log.Info($"ThreesomeAnimation: ID: {__instance.SexID}, Type: {__instance.SexType}");
        RandomReverseMod.Apply(__instance);
    }
}