using HarmonyLib;
using Il2Cpp;

namespace YC.GameTrainerMod.Patches;
internal class CharacterAttributesPatch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterAttributes), nameof(CharacterAttributes.ReduceHealth))]
    [HarmonyPatch(typeof(CharacterAttributes), nameof(CharacterAttributes.ReduceWillpower))]
    static bool CharacterAttributesReduceHealthAndWillpowerPrefix(CharacterAttributes __instance, bool __runOriginal, ref int ammount)
    {
        if (GameTrainerMod.IsGodMode && (__instance.isPlayer || __instance.combatAI.isAlly))
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
