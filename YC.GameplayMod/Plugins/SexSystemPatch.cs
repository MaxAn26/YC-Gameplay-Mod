using System;

using BaseMod.Core.Extensions;

using HarmonyLib;

using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Plugins;
internal class SexSystemPatch {
    internal static bool Prepare() {
        try {
            if (!SexChoiceRealismMod.IsModActive && !RandomReverseMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.LogWarning($"{nameof(SexSystemPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexSystem), nameof(SexSystem.SetHeavyBondageAnimation))]
    static bool SexSystemSetHeavyBondageAnimationPrefix(bool __runOriginal) {
        SexChoiceRealismMod.SetSexID();

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexSystem), nameof(SexSystem.SetSexAnimation))]
    static bool SexSystemSetSexAnimationPrefix(bool __runOriginal) {
        SexChoiceRealismMod.SetSexID();

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexSystem), nameof(SexSystem.SetThreesomeAnimation))]
    static bool SexSystemSetThreesomeAnimationPrefix(bool __runOriginal) {
        SexChoiceRealismMod.SetThreesomeSexID();

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexSystem), nameof(SexSystem.SetHeavyBondageAnimation))]
    static void SexSystemSetHeavyBondageAnimationPostfix(SexSystem __instance) {
        Plugin.Log.Info($"HeavyBondageAnimation: ID: {SexSystem.SexID}, Type: {__instance.SexType}");
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexSystem), nameof(SexSystem.SetSexAnimation))]
    static void SexSystemSetSexAnimationPostfix(SexSystem __instance) {
        Plugin.Log.Info($"SexAnimation: ID: {SexSystem.SexID}, Type: {__instance.SexType}");
        RandomReverseMod.Apply(__instance);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SexSystem), nameof(SexSystem.SetThreesomeAnimation))]
    static void SexSystemSetThreesomeAnimationPostfix(SexSystem __instance) {
        Plugin.Log.Info($"ThreesomeAnimation: ID: {SexSystem.SexID}, Type: {__instance.SexType}");
        RandomReverseMod.Apply(__instance);
    }
}