using HarmonyLib;
using Il2Cpp;
using YC.GameplayMod.Components;
using YC.GameplayMod.Mods;

namespace YC.GameplayMod.Patches;

[HarmonyPatch(typeof(CharacterSex))]
internal class CharacterSexPatch
{
    internal static bool Prepare()
    {
        try
        {
            if (!DickStraponVisibilityMod.IsModActive)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            Core.LogWarning($"{nameof(CharacterSexPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(CharacterSex.Start))]
    static void CharacterSexStartPostfix(CharacterSex __instance)
    {
        if (!string.IsNullOrWhiteSpace(__instance.characterName))
        {
            GameplayModComponent.RegisterClass(__instance);
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(nameof(CharacterSex.SetDick))]
    static void CharacterSexSetDickPostfix(CharacterSex __instance, bool dickVisibility) => DickStraponVisibilityMod.SetDick(__instance, dickVisibility);
}
