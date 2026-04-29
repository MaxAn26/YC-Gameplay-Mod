using System.Collections.Generic;
using System.IO;

using HarmonyLib;

using Il2Cpp;

using Il2CppMagicaCloth2;

using Il2CppTriangleNet.Meshing.Algorithm;

using MelonLoader.Utils;

using UnityEngine;

using static Il2CppMagicaCloth2.Define;

namespace YC.GameplayMod.Patches;
internal class DebugPatch
{
    private static string _logFile = "GameLog.txt";

    internal static bool Prepare()
    {
        try
        {
            _logFile = $"GameLog-{System.DateTime.Now:dd_MM_yy-HH_mm_ss}.txt";

            return true;
        }
        catch (System.Exception)
        {
            Plugin.Log.Warn($"{nameof(DebugPatch)} not applied due exeption");
            return false;
        }
    }

#if DEBUG
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Debug), nameof(Debug.Log), [typeof(Il2CppSystem.Object)])]
    [HarmonyPatch(typeof(Debug), nameof(Debug.LogWarning), [typeof(Il2CppSystem.Object)])]
    [HarmonyPatch(typeof(Debug), nameof(Debug.LogError), [typeof(Il2CppSystem.Object)])]
    [HarmonyPatch(typeof(Debug), nameof(Debug.LogAssertion), [typeof(Il2CppSystem.Object)])]
    static bool DebugLogPrefix(bool __runOriginal, ref Il2CppSystem.Object message)
    {
        string log = Path.Combine(MelonEnvironment.MelonLoaderLogsDirectory, _logFile);

        File.AppendAllText(log, $"[{System.DateTime.Now:HH:mm:ss}] {message.ToString()}\r\n");

        if (!__runOriginal)
        {
            return false;
        }

        return true;
    }

    /*[HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(WardrobeLibrary), nameof(WardrobeLibrary.ApplyHair))]
    static bool WardrobeLibraryApplyHairPrefix(WardrobeLibrary __instance, bool __runOriginal, ref HairSet hairSet, HairLibrary library, int hairIndex, Color hairColor, float hairAlpha, bool isEnemy, GameObject owner, List<ColliderComponent> colliders, bool isSetB, Material sourceMat) {
        Plugin.Log.Debug( $"{hairSet}, {library}, {hairIndex}, {hairColor}, {hairAlpha}, {(isEnemy ? "enemy" : "not enemy")}, {owner.name}, {(isSetB ? "SetB" : "SetA")}, {sourceMat.name}" );

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(WardrobeLibrary), nameof(WardrobeLibrary.ApplyHair))]
    static void WardrobeLibraryApplyHairPostfix(WardrobeLibrary __instance, ref HairSet hairSet, HairLibrary library, int hairIndex, Color hairColor, float hairAlpha, bool isEnemy, GameObject owner, List<ColliderComponent> colliders, bool isSetB, Material sourceMat) {
        Plugin.Log.Debug($"{hairSet.hairMat.name}, {library}, {hairIndex}, {hairColor}, {hairAlpha}, {(isEnemy ? "enemy" : "not enemy")}, {owner.name}, {(isSetB ? "SetB" : "SetA")}, {sourceMat.name}");
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(WardrobeLibrary), nameof(WardrobeLibrary.GetEnemyHairMaterial))]
    static void WardrobeLibraryGetEnemyHairMaterialPostfix(WardrobeLibrary __instance, Material __result, int hairIndex, Color hairColor, float hairAlpha, Material source) {
        Plugin.Log.Debug($"{hairIndex}, {hairColor}, {hairAlpha}, {source.name}");
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(WardrobeLibrary), nameof(WardrobeLibrary.GetPlayerHairMaterial))]
    static void WardrobeLibraryGetPlayerHairMaterialPostfix(WardrobeLibrary __instance, Material __result, GameObject owner, Color hairColor, float hairAlpha, Material source) {
        Plugin.Log.Debug($"{owner.name}, {hairColor}, {hairAlpha}, {source.name}");
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(WardrobeLibrary), nameof(WardrobeLibrary.GetPlayerHairBMaterial))]
    static void WardrobeLibraryGetPlayerHairBMaterialPostfix(WardrobeLibrary __instance, Material __result, GameObject owner, Color hairColor, float hairAlpha, Material source) {
        Plugin.Log.Debug($"{owner.name}, {hairColor}, {hairAlpha}, {source.name}");
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe), "set_HairSetA", MethodType.Setter)]
    static void WardrobeHairSetAPostfix(Wardrobe __instance, HairSet value) {
        Plugin.Log.Debug($"{value.hairMat.name}");
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Wardrobe), "set_HairSetA", MethodType.Setter)]
    static void WardrobeHairSetBPostfix(Wardrobe __instance, HairSet value) {
        Plugin.Log.Debug($"{value.hairMat.name}");
    }*/

    /*[HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(SkinnedMeshRenderer), nameof(SkinnedMeshRenderer.SetBlendShapeWeight))]
    static bool SkinnedMeshRendererSetBlendShapeWeightPrefix(SkinnedMeshRenderer __instance, bool __runOriginal, int index, float value) {
        if (!__instance.name.StartsWith("Hidden") && !__instance.name.StartsWith("Zeit") && !__instance.name.StartsWith("Stencil"))
            Plugin.Log.Debug($"{__instance.name}: index: {index}, value: {value}");

        if (!__runOriginal)
            return false;

        return true;
    }*/

    /*[HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Material), nameof(Material.SetColor), [typeof(string), typeof(Color)])]
    static bool MaterialSetColorPrefix(Material __instance, bool __runOriginal, string name, Color value) {
        if (__instance.name.StartsWith("G3") || __instance.name.StartsWith("M3") || __instance.name.StartsWith("Skin"))
            Plugin.Log.Debug($"{__instance.name} set {name} = {value}");

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Material), nameof(Material.SetTexture), [typeof(string), typeof(Texture)])]
    static bool MaterialSetTexturePrefix(Material __instance, bool __runOriginal, string name, Texture value) {
        if (name.StartsWith("_Make"))
            Plugin.Log.Debug($"{__instance.name} set {name} = {value.name}");

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Material), nameof(Material.SetFloat), [typeof(string), typeof(float)])]
    static bool MaterialSetFloatPrefix(Material __instance, bool __runOriginal, string name, float value) {
        if (__instance.name.StartsWith("G3") || __instance.name.StartsWith("M3") || __instance.name.StartsWith("Skin"))
            Plugin.Log.Debug($"{__instance.name} set {name} = {value}");

        if (!__runOriginal)
            return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Material), nameof(Material.SetFloat), [typeof(int), typeof(float)])]
    static bool MaterialSetFloatPrefix(Material __instance, bool __runOriginal, int nameID, float value) {
        if (!__instance.name.StartsWith("Hidden") && !__instance.name.StartsWith("Zeit") && !__instance.name.StartsWith("Stencil"))
            Plugin.Log.Debug($"{__instance.name} set {nameID} = {value}");

        if (!__runOriginal)
            return false;

        return true;
    }*/
#endif
}
