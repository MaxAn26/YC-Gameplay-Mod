using System;

using HarmonyLib;

using Il2Cpp;

namespace YC.GameTrainerMod.Patches;
internal class CombatTalentInventoryPatch
{
    internal static bool Prepare()
    {
        try
        {

            return true;
        }
        catch (Exception)
        {
            Plugin.Log.Warn($"{nameof(CombatTalentInventoryPatch)} not applied due exeption");
            return false;
        }
    }

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
