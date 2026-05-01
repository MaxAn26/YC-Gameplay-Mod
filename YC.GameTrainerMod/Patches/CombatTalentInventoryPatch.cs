using HarmonyLib;
using Il2Cpp;

namespace YC.GameTrainerMod.Patches;
internal class CombatTalentInventoryPatch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CombatTalentInventory), nameof(CombatTalentInventory.HasTalent), [typeof(int)])]
    static void CombatTalentInventoryHasTalentPostfix(ref bool __result, int id)
    {
        if (id is >= 505 and <= 508)
        {
            __result = false;
        }
    }
}
