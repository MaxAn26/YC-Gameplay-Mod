using System;

using HarmonyLib;

using Il2Cpp;

namespace YC.GameplayMod.Patches;
internal class CharacterAttributesPatch {
    internal static bool Prepare() {
        try {


            return true;
        } catch (Exception) {
            Plugin.Log.Warn($"{nameof(CharacterAttributesPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterAttributes), nameof(CharacterAttributes.SetupSexPositions))]
    static void CharacterAttributesSetupSexPositionsPostfix(CharacterAttributes __instance) {
        Plugin.Log.Info($"Setup Sex Positions for '{__instance.characterSex.characterName}'");
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterAttributes), nameof(CharacterAttributes.Death))]
    static void CharacterAttributesDeathPostfix(CharacterAttributes __instance) {
        CharacterAttributes.print($"{__instance.characterName} is DEAD");
    }
}