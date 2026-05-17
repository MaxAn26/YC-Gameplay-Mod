using HarmonyLib;
using Il2Cpp;

namespace YC.GameTrainerMod.Patches;

[HarmonyPatch(typeof(CharacterAttributes))]
internal class CharacterAttributesPatch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(CharacterAttributes.ReduceHealth))]
    [HarmonyPatch(nameof(CharacterAttributes.ReduceWillpower))]
    static bool CharacterAttributesReduceHealthAndWillpowerPrefix(CharacterAttributes __instance, bool __runOriginal, ref int ammount)
    {
        if (Core.IsGodMode && (__instance.isPlayer || __instance.combatAI.isAlly))
        {
            ammount = 0;
        }

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }
}
