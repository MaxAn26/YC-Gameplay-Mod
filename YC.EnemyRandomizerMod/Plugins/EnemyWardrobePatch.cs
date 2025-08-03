using System;

using BaseMod.Core.Extensions;

using HarmonyLib;

using YC.EnemyRandomizerMod.Mods;

namespace YC.EnemyRandomizerMod.Plugins;
internal class EnemyWardrobePatch {
    internal static bool Prepare() {
        try {
            if (!EnemyBodyRandomizerMod.IsModActive)
                return false;

            return true;
        } catch (Exception) {
            Plugin.Log.LogWarning($"{nameof(EnemyWardrobePatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterSex), nameof(CharacterSex.Start))]
    static void CharacterSexStartPostfix(CharacterSex __instance) {
        if (string.IsNullOrWhiteSpace(__instance.characterName) || __instance.IsPlayer)
            return;

        if (__instance.gameObject.TryGetComponentWithCast(out EnemyWardrobe enemyWardrobe))             EnemyBodyRandomizerMod.Apply(enemyWardrobe);
    }

    /*[HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Material), nameof(Material.SetFloat), [typeof(string), typeof(float)])]
    static bool Class1MethodNamePrefix(Material __instance, string __0, float __1, bool __runOriginal) {
        if (!__instance.name.Contains("Hidden"))
            Plugin.Log.Message($"Name: {__instance.name}, Key: {__0}, Value: {__1}");

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Material), nameof(Material.SetColor), [typeof(string), typeof(Color)])]
    static bool MaterialSetColorPrefix(Material __instance, string __0, Color __1, bool __runOriginal) {
        if (!__instance.name.Contains("Hidden"))
            Plugin.Log.Message($"Name: {__instance.name}, Key: {__0}, Color: {__1}");

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SkinnedMeshRenderer), nameof(SkinnedMeshRenderer.SetBlendShapeWeight))]
    static void SkinnedMeshRendererMethodNamePostfix(SkinnedMeshRenderer __instance, int __0, float __1) {
        Plugin.Log.Message($"Name: {__instance.name}, Index: {__0}, Value: {__1}");
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(MaterialPropertyBlock), nameof(MaterialPropertyBlock.SetFloat), [typeof(string), typeof(float)])]
    static bool MaterialPropertyBlockSetFloatPrefix(MaterialPropertyBlock __instance, string __0, float __1, bool __runOriginal) {
        Plugin.Log.Message($"Key: {__0}, Color: {__1}");

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Material), nameof(Material.SetTexture), [typeof(string), typeof(Texture)])]
    static bool MaterialSetTexturePrefix(Material __instance, string __0, Texture __1, bool __runOriginal) {
        if (!__instance.name.Contains("Hidden") && !__instance.name.Contains("Sky"))
            Plugin.Log.Message($"Name: {__instance.name}, Key: {__0}, Texture: {__1}");

        if (!__runOriginal)
            return false;

        return true;
    }*/
}