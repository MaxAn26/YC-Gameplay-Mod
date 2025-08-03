using System;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using HarmonyLib;

using UnityEngine;

namespace YC.GameplayMod.Plugins;
internal class CharacterSexPatch {
    internal static float lastVal = 0.8f;

    internal static bool Prepare() {
        try {


            return true;
        } catch (Exception) {
            Plugin.Log.LogWarning($"{nameof(CharacterSexPatch)} not applied due exeption");
            return false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(CharacterSex), nameof(CharacterSex.Start))]
    static void CharacterSexStartPostfix(CharacterSex __instance) {
        if (__instance.IsPlayer)
            return;

        Plugin.Log.Info($"CharacterSex Start");
        var bones = __instance.gameObject.GetComponentsInChildren<DynamicBone>();
        if (bones.Count > 0) {
            //float size = RandomUtils.Float(1.0f, 1.4f);
            float size = Mathf.Min(lastVal + 0.3f, 2.0f);
            Plugin.Log.Info($"CharacterSex Sex BOOB as {size}");
            var a = bones[0].m_Root;
            var scale = a.localScale;
            scale.x = size;
            scale.y = size;
            scale.z = size;
            foreach (var item in bones) {
                item.m_Root.localScale = scale;
            }
            lastVal = size < 2.0f ? size : 0.8f;
        }
    }
}