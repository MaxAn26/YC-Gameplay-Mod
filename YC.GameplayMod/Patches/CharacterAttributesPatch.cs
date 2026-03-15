using System;

using HarmonyLib;

using Il2Cpp;

using YC.GameplayMod.Components;
using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;
internal class CharacterAttributesPatch {
    internal static bool Prepare() {
        try {
            if (!SexChoiceRealismMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.Warn($"{nameof(CharacterAttributesPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterAttributes), nameof(CharacterAttributes.Initialize))]
    [HarmonyPatch(typeof(CharacterAttributes), nameof(CharacterAttributes.InitializeAlly))]
    static void CharacterAttributesInitializePostfix(CharacterAttributes __instance) {
        if (!string.IsNullOrWhiteSpace(__instance.characterSex.characterName))
            GameplayModComponent.RegisterClass(__instance);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterAttributes), nameof(CharacterAttributes.Rest))]
    static void CharacterAttributesRestPostfix(CharacterAttributes __instance) {
        SexChoiceRealismMod.ResetSexCount( __instance );
    }
}
