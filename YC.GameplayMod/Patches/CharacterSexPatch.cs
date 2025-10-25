using System;

using HarmonyLib;

using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;
internal class CharacterSexPatch {
    internal static bool Prepare() {
        try {
            if (!DickStraponVisibilityMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.LogWarning($"{nameof(CharacterSexPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterSex), nameof(CharacterSex.SetDick))]
    static void CharacterSexSetDickPostfix(CharacterSex __instance, bool __0) {
        DickStraponVisibilityMod.SetDick(__instance, __0);
    }
}