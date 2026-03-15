using System;

using HarmonyLib;

using Il2Cpp;

using YC.GameplayMod.Components;
using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;
internal class CharacterSexPatch {
    internal static bool Prepare() {
        try {
            if (!DickStraponVisibilityMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.Warn($"{nameof(CharacterSexPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterSex), nameof(CharacterSex.Start))]
    static void CharacterSexStartPostfix(CharacterSex __instance) {
        if (!string.IsNullOrWhiteSpace( __instance.characterName ))
            GameplayModComponent.RegisterClass(__instance);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterSex), nameof(CharacterSex.SetDick))]
    static void CharacterSexSetDickPostfix(CharacterSex __instance, bool __0) {
        DickStraponVisibilityMod.SetDick(__instance, __0);
    }
}
