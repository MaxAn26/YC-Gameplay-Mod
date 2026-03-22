using System;

using HarmonyLib;

using Il2Cpp;

using YC.GameTrainerMod.Components;

namespace YC.GameTrainerMod.Patches;
internal class CharacterAttributesPatch {
    internal static bool Prepare() {
        try {
            
            return true;
        } catch (Exception) {
            Plugin.Log.Warn($"{nameof(CharacterAttributesPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterAttributes), nameof(CharacterAttributes.ReduceHealth))]
    [HarmonyPatch(typeof(CharacterAttributes), nameof(CharacterAttributes.ReduceWillpower))]
    static bool CharacterAttributesReduceHealthAndWillpowerPrefix(CharacterAttributes __instance, bool __runOriginal, ref int ammount) {
        if (GameTrainerComponent.IsGodMode && (__instance.isPlayer || __instance.combatAI.isAlly))
            ammount = 0;

        if (!__runOriginal)
            return false;

        return true;
    }
}
