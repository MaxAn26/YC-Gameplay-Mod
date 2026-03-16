using System;
using System.Collections.Generic;

using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using Il2Cpp;

using MelonLoader;

using UnityEngine;

using YC.GameplayMod.Components;
using YC.GameplayMod.Models;

namespace YC.GameplayMod.Mods;
public class EnemyBodyRandomizerMod {
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> RandomizeCompanions;
    internal static MelonPreferences_Entry<int> ChanceForFuta;
    internal static MelonPreferences_Entry<int> ChanceForFullFuta;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    #region Storage
    public static CharacterDataa Character => CharacterDataa.Instance;
    #endregion

    public static void Load(PluginConfig config) {
        try {
            Enabled = config.Entry(nameof(EnemyBodyRandomizerMod), nameof(Enabled), false, 
                "Activates the modification", new PluginConfig.AcceptableValueList<bool>([true, false]));
            RandomizeCompanions = config.Entry(nameof(EnemyBodyRandomizerMod), nameof(RandomizeCompanions), false, 
                "Randomize player companions", new PluginConfig.AcceptableValueList<bool>([true, false]));
            ChanceForFuta = config.Entry(nameof(EnemyBodyRandomizerMod), nameof(ChanceForFuta), 35, 
                "Chance for female character with active or mixed role become futanari", new PluginConfig.AcceptableValueRange<int>(0, 100));
            ChanceForFullFuta = config.Entry(nameof(EnemyBodyRandomizerMod), nameof(ChanceForFullFuta), 50, 
                "Chance for female futa character get full futa (dick + balls)", new PluginConfig.AcceptableValueRange<int>(0, 100));

        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    public static void Apply(CombatEnemyManager combatEnemyManager, CharacterSex characterSex, Wardrobe wardrobe) {
        try {
            if (!Enabled.Value)
                return;

            if (Character.adultSettingsDATA.EREnabled || wardrobe.enemyData is null)
                return;

            if (combatEnemyManager.requiredAllies.Contains(characterSex.characterName) && !RandomizeCompanions.Value) {
                SetEnemyDickType(wardrobe, characterSex);
                CheckHat(wardrobe);
                return;
            }

            Material skin   = UnityEngine.Object.Instantiate(wardrobe.SkinCharacter.sharedMaterials[0]);
            Material face   = UnityEngine.Object.Instantiate(wardrobe.SkinCharacter.sharedMaterials[1]);
            Material eyes   = UnityEngine.Object.Instantiate(wardrobe.SkinCharacter.sharedMaterials[2]);
            Material beard  = UnityEngine.Object.Instantiate(wardrobe.SkinCharacter.sharedMaterials[3]);
            var materials = wardrobe.SkinCharacter.materials;
            materials[0] = skin;
            materials[1] = face;
            materials[2] = eyes;
            materials[3] = beard;
            wardrobe.SkinCharacter.materials = materials;

            SetHair(wardrobe);
            SetFaceSize(wardrobe, characterSex);
            SetEnemyDickType(wardrobe, characterSex);
            SetBodySize(wardrobe, characterSex);
            SetColors(wardrobe);
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return;
        }
    }

    public static void SetHair(Wardrobe wardrobe) {
        if (wardrobe.HairMeshes.Count <= 0)
            return;

        CheckHat(wardrobe);

        var hairMesh = wardrobe.characterSex.IsMale
            ? RandomUtils.Int32( 0, 16 )
            : RandomUtils.Int32( 17, wardrobe.HairMeshes.Count - 1 );


        if (wardrobe.HairMeshFilter.mesh != wardrobe.HatHair)             wardrobe.HairMeshFilter.mesh = wardrobe.HairMeshes[hairMesh];
        wardrobe.HairMeshRenderer.sharedMaterial.SetFloat("_AlphaClipThreshold", 0.0f);
        wardrobe.HairMeshRenderer.sharedMaterial.SetFloat("_AnisotropyValue", RandomUtils.Float(0.5f, 0.95f));
    }

    public static void SetFaceSize(Wardrobe enemyWardrobe, CharacterSex characterSex) {
        #region Face Size
        int face = RandomUtils.Int32(0, 20);
        Plugin.Log.Info( $"Face: {face}");
        switch (face) {
            case 1:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 2:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 3:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 4:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 5:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 6:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 7:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 8:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 9:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 10:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 35.0f);
                break;
            case 11:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 70.0f);
                break;
            case 12:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 100.0f);
                break;
            case 13:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 14:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 15:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 16:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
            case 17:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 35.0f);
                break;
            case 18:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 50.0f);
                break;
            case 19:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 35.0f);
                break;
            case 20:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 50.0f);
                break;
            default:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(2, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(3, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(4, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(5, 0.0f);
                break;
        }
        #endregion

        #region Eyes Style
        int eyesStyle = RandomUtils.Int32(0, 20);
        Plugin.Log.Info($"Eyes Style: {eyesStyle}");
        switch (eyesStyle) {
            case 1:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 2:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 3:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 4:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 5:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 6:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 7:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 8:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 9:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 10:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 35.0f);
                break;
            case 11:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 70.0f);
                break;
            case 12:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 100.0f);
                break;
            case 13:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 14:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 15:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 16:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
            case 17:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 35.0f);
                break;
            case 18:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 50.0f);
                break;
            case 19:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 35.0f);
                break;
            case 20:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 50.0f);
                break;
            default:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(6, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(7, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(8, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(9, 0.0f);
                break;
        }
        #endregion

        #region Nose Style
        int noseStyle = RandomUtils.Int32(0, 12);
        Plugin.Log.Info($"Nose Style: {noseStyle}");
        switch (noseStyle) {
            case 1:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 0.0f);
                break;
            case 2:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 0.0f);
                break;
            case 3:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 0.0f);
                break;
            case 4:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 0.0f);
                break;
            case 5:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 0.0f);
                break;
            case 6:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 0.0f);
                break;
            case 7:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 0.0f);
                break;
            case 8:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 0.0f);
                break;
            case 9:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 0.0f);
                break;
            case 10:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 35.0f);
                break;
            case 11:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 70.0f);
                break;
            case 12:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 100.0f);
                break;
            default:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(11, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(12, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(13, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(14, 0.0f);
                break;
        }
        #endregion

        #region Brow Style
        int browStyle = RandomUtils.Int32(0, 8);
        Plugin.Log.Info($"Brow Style: {browStyle}");
        switch (browStyle) {
            case 1:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(24, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(25, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(26, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(27, 0.0f);
                break;
            case 2:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(24, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(25, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(26, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(27, 0.0f);
                break;
            case 3:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(24, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(25, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(26, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(27, 0.0f);
                break;
            case 4:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(24, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(25, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(26, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(27, 0.0f);
                break;
            case 5:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(24, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(25, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(26, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(27, 0.0f);
                break;
            case 6:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(24, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(25, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(26, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(27, 0.0f);
                break;
            case 7:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(24, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(25, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(26, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(27, 50.0f);
                break;
            case 8:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(24, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(25, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(26, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(27, 100.0f);
                break;
            default:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(24, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(25, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(26, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(27, 0.0f);
                break;
        }
        #endregion

        #region Mouth Style
        int mouthStyle = RandomUtils.Int32(0, 20);
        Plugin.Log.Info($"Mouth Style: {mouthStyle}");
        switch (mouthStyle) {
            case 1:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 2:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 3:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 4:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 5:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 6:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 7:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 8:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 70.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 9:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 10:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 35.0f);
                break;
            case 11:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 70.0f);
                break;
            case 12:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 100.0f);
                break;
            case 13:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 14:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 15:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 16:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
            case 17:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 35.0f);
                break;
            case 18:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 50.0f);
                break;
            case 19:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 35.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 35.0f);
                break;
            case 20:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 50.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 50.0f);
                break;
            default:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(16, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(17, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(18, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(19, 0.0f);
                break;
        }
        #endregion

        #region Mouth Length
        int mouthLenght = RandomUtils.Int32(0, 10);
        Plugin.Log.Info($"Mouth Lenght: {mouthLenght}");
        switch (mouthLenght) {
            case 1:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(20, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(21, 20.0f);
                break;
            case 2:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(20, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(21, 40.0f);
                break;
            case 3:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(20, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(21, 60.0f);
                break;
            case 4:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(20, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(21, 80.0f);
                break;
            case 5:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(20, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(21, 100.0f);
                break;
            case 6:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(20, 20.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(21, 0.0f);
                break;
            case 7:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(20, 40.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(21, 0.0f);
                break;
            case 8:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(20, 60.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(21, 0.0f);
                break;
            case 9:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(20, 80.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(21, 0.0f);
                break;
            case 10:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(20, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(21, 0.0f);
                break;
            default:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(20, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(21, 0.0f);
                break;
        }
        #endregion

        #region Lips Forward
        int lipsForward = RandomUtils.Int32(0, 10);
        Plugin.Log.Info($"Lips Forward: {lipsForward}");
        switch (lipsForward) {
            case 1:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(35, 10.0f);
                break;
            case 2:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(35, 20.0f);         
                break;
            case 3:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(35, 30.0f);         
                break;
            case 4:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(35, 40.0f);         
                break;
            case 5:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(35, 50.0f);         
                break;
            case 6:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(35, 60.0f);         
                break;
            case 7:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(35, 70.0f);         
                break;
            case 8:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(35, 80.0f);         
                break;
            case 9:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(35, 90.0f);
                break;
            case 10:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(35, 100.0f);
                break;
            default:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(35, 0.0f);         
                break;
        }
        #endregion

        #region Lips Size
        int lipsSize = RandomUtils.Int32(0, 8);
        Plugin.Log.Info($"Lips Size: {lipsSize}");
        switch (lipsSize) {
            case 1:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(22, 12.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(23, 12.0f);
                break;
            case 2:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(22, 24.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(23, 24.0f);
                break;
            case 3:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(22, 36.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(23, 36.0f);
                break;
            case 4:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(22, 48.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(23, 48.0f);
                break;
            case 5:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(22, 54.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(23, 54.0f);
                break;
            case 6:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(22, 66.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(23, 66.0f);
                break;
            case 7:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(22, 88.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(23, 88.0f);
                break;
            case 8:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(22, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(23, 100.0f);
                break;
            default:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(22, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(23, 0.0f);
                break;
        }
        #endregion

        #region Ears Style
        int earsStyle = enemyWardrobe.enemyData.statsDATA.EnemyEthnicity > 5 ? RandomUtils.Int32(1, 10) : 0;
        Plugin.Log.Info($"Ears Style: {earsStyle}");
        switch (earsStyle) {
            case 1:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(28, 20.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(29, 0.0f);
                break;
            case 2:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(28, 40.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(29, 0.0f);
                break;
            case 3:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(28, 60.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(29, 0.0f);
                break;
            case 4:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(28, 80.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(29, 0.0f);
                break;
            case 5:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(28, 100.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(29, 0.0f);
                break;
            case 6:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(28, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(29, 20.0f);
                break;
            case 7:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(28, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(29, 40.0f);
                break;
            case 8:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(28, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(29, 60.0f);
                break;
            case 9:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(28, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(29, 80.0f);
                break;
            case 10:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(28, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(29, 100.0f);
                break;
            default:
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(28, 0.0f);
                enemyWardrobe.SkinCharacter.SetBlendShapeWeight(29, 0.0f);
                break;
        }
        #endregion

        if (characterSex.IsMale)
            enemyWardrobe.SkinCharacter.SetBlendShapeWeight(1, 100f);

        enemyWardrobe.SkinCharacter.materials[1].SetColor("_Mask1_Gchannel_ColorAmountA", new Color { r = 0.596f, g = 0f, b = 0.129f, a = GetSkewedValue(0.5f) });
    }

    public static void SetBodySize(Wardrobe enemyWardrobe, CharacterSex characterSex) {
        var profile = RandomUtils.Item( GetBodyProfiles());
        float minBoobsKoeff = -0.25f;
        float maxBoobsKoeff = 0.25f;
        if (!characterSex.IsPlayer && !characterSex.IsMale && RandomUtils.Chance(25)) {
            Plugin.Log.Info("Extra boobs");
            minBoobsKoeff += 0.5f;
            maxBoobsKoeff += 0.75f;
        }

        float minBootyKoeff = -0.25f;
        float maxBootyKoeff = 0.25f;
        if (!characterSex.IsPlayer && !characterSex.IsMale && RandomUtils.Chance(25)) {
            Plugin.Log.Info("Extra booty");
            minBootyKoeff += 0.5f;
            maxBoobsKoeff += 0.75f;
        }

        var body = new CharacterBody {
            Muscle  = Math.Clamp(profile.Muscle + RandomUtils.Float( -0.3f, 0.3f ) , 0.0f, 2.5f),
            Torso   = Math.Clamp(profile.Torso + RandomUtils.Float( -(profile.Torso * 0.15f), profile.Torso * 0.15f ) , 0.3f, 2.5f ),
            Hips    = Math.Clamp(profile.Hips + RandomUtils.Float( -(profile.Hips * 0.15f), profile.Hips * 0.15f ) , 0.3f, 3.5f ),
            Belly   = Math.Clamp(profile.Belly + RandomUtils.Float( -(profile.Belly * 0.15f), profile.Belly * 0.15f ) , 0.3f, 4.0f ),
            Arms    = Math.Clamp(profile.Arms + RandomUtils.Float( -(profile.Arms * 0.15f), profile.Arms * 0.15f ) , 0.3f, 5.0f ),
            Biceps  = Math.Clamp(profile.Biceps + RandomUtils.Float( -(profile.Biceps * 0.15f), profile.Biceps * 0.15f ), 0.3f, 7.0f ),
            Thighs  = Math.Clamp(profile.Thighs + RandomUtils.Float( -(profile.Thighs * 0.15f), profile.Thighs * 0.15f ), 0.3f, 4.5f ),
            Calves  = Math.Clamp(profile.Calves + RandomUtils.Float( -(profile.Calves * 0.15f), profile.Calves * 0.15f ), 0.3f, 4.5f ),
            Boobs   = characterSex.IsMale
                        ? 1.0f
                        : Math.Clamp(profile.Boobs + RandomUtils.Float( minBoobsKoeff, maxBoobsKoeff ) , 0.7f, 1.5f),
            AreolaIndex = Math.Clamp(profile.AreolaSize - 1 + RandomUtils.Int32(-1, 1), 0, 7),
            Booty   = Math.Clamp(profile.Booty + RandomUtils.Float( minBootyKoeff, maxBootyKoeff ) , 0.5f, 1.5f ),
            Dick    = characterSex.IsMale
                        ? Math.Clamp(profile.Dick + RandomUtils.Float( -(profile.Dick * 0.15f), profile.Dick * 0.15f ) , 0.7f, 1.5f )
                        : Math.Clamp(profile.Dick + RandomUtils.Float( -(profile.Dick * 0.15f), profile.Dick * 0.15f ) , 0.7f, 1.5f )
        };

        Plugin.Log.Info($"Set body params for character {characterSex.characterName}. Profile: {profile.Name}");
        enemyWardrobe.SkinCharacter.materials[0].SetFloat("_FinalNormalMapPower", body.Muscle);
        float smoothness = RandomUtils.Float(0.0f, 0.9f);
        enemyWardrobe.SkinCharacter.material.SetFloat("_SmoothnessDeviate", smoothness);
        enemyWardrobe.SkinCharacter.materials[1].SetFloat("_SmoothnessDeviate", smoothness);
        enemyWardrobe.SkinDick.material.SetFloat("_SmoothnessDeviate", smoothness);

        if (!characterSex.IsMale) {
            var wardrobe2 = GameObject.Find("WardrobeOBJ")?.GetComponentWithCast<Wardrobe2>();
            if (wardrobe2 is not null) {
                int ind = Math.Clamp(body.AreolaIndex, 0, wardrobe2.MakeupBodyTex.Count - 1);
                enemyWardrobe.SkinCharacter.materials[0].SetTexture("_MakeUpMask1_RGB", wardrobe2.MakeupBodyTex[ind]); 
            }
        }

        var back = new Vector3 {
            x = body.Torso,
            y = body.Torso,
            z = body.Torso
        };
        enemyWardrobe.Back.transform.localScale = back;

        var waist = new Vector3 {
            x = body.Hips,
            y = body.Hips,
            z = body.Hips
        };
        enemyWardrobe.Waist.transform.localScale = waist;

        var belly = new Vector3 {
            x = body.Belly,
            y = body.Belly,
            z = body.Belly
        };
        enemyWardrobe.Belly.transform.localScale = belly;

        var arms = new Vector3 {
            x = body.Arms,
            y = body.Arms,
            z = body.Arms
        };
        enemyWardrobe.LeftArm.transform.localScale = arms;
        enemyWardrobe.RightArm.transform.localScale = arms;

        var biceps = new Vector3 {
            x = body.Biceps,
            y = body.Biceps,
            z = body.Biceps
        };
        enemyWardrobe.LeftShoulder.transform.localScale = biceps;
        enemyWardrobe.RightShoulder.transform.localScale = biceps;

        var thighs = new Vector3 {
            x = body.Thighs,
            y = body.Thighs,
            z = body.Thighs
        };
        enemyWardrobe.LeftThigh.transform.localScale = thighs;
        enemyWardrobe.RightThigh.transform.localScale = thighs;

        var calves = new Vector3 {
            x = body.Calves,
            y = body.Calves,
            z = body.Calves
        };
        enemyWardrobe.LeftLeg.transform.localScale = calves;
        enemyWardrobe.RightLeg.transform.localScale = calves;

        var boobs = new Vector3 {
            x = body.Boobs,
            y = body.Boobs,
            z = body.Boobs
        };
        enemyWardrobe.LeftBoob.transform.localScale = boobs;
        enemyWardrobe.RightBoob.transform.localScale = boobs;

        var booty = new Vector3 {
            x = body.Booty,
            y = body.Booty,
            z = body.Booty
        };
        enemyWardrobe.LeftBooty.transform.localScale = booty;
        enemyWardrobe.RightBooty.transform.localScale = booty;

        var dick = new Vector3 {
            x = body.Dick,
            y = body.Dick,
            z = body.Dick
        };
        enemyWardrobe.Dick.transform.localScale = dick;
    }

    public static void SetColors(Wardrobe wardrobe) {
        var wardrobe2 = GameObject.Find("WardrobeOBJ")?.GetComponentWithCast<Wardrobe2>();
        if (wardrobe2 is null)
            return;

        int skinId = wardrobe.enemyData.statsDATA.EnemyEthnicity switch {
            0 => RandomUtils.Int32(13),
            1 or 4 or 6 => RandomUtils.Int32(0, 4),
            2 or 7 => RandomUtils.Int32(3, 8),
            5 => RandomUtils.Int32(13, wardrobe2.SkinTones.Count - 1),
            8 => RandomUtils.Int32(17, wardrobe2.SkinTones.Count - 1),
            9 => RandomUtils.Int32(13, 16),
            _ => RandomUtils.Int32(0, wardrobe2.SkinTones.Count - 1)
        };

        int makeUpId, lipsId;

        switch (wardrobe.enemyData.statsDATA.EnemyMakeup) {
            case 0 or 5 or 6:
                makeUpId    = RandomUtils.Int32(wardrobe2.MakeupColors.Count - 1);
                lipsId      = RandomUtils.Int32(wardrobe2.MakeupColors.Count - 1);
                break;
            case 2 or 3 or 4:
                makeUpId    = RandomUtils.Int32(7);
                lipsId      = RandomUtils.Int32(7);
                break;
            default:
                makeUpId    = 0;
                lipsId      = 0;
                break;
        }

        int makeUpIntense = wardrobe.enemyData.statsDATA.EnemyMakeup switch {
            0 => RandomUtils.Int32(1, 3),
            2 or 3 or 4 => wardrobe.enemyData.statsDATA.EnemyMakeup - 1,
            _ => 0
        };

        int eyesId = RandomUtils.Chance(15)
            ? RandomUtils.Int32(21, wardrobe2.EyeColors.Count - 1)
            : RandomUtils.Int32(20);

        int hairId = RandomUtils.Chance(15)
            ? RandomUtils.Int32(29, wardrobe2.HairColors.Count - 1)
            : RandomUtils.Int32(28);

        Color skinColor = wardrobe2.SkinTones[skinId];
        Color eyesColor = wardrobe2.EyeColors[eyesId];
        Color hairColor = wardrobe2.HairColors[hairId];
        
        wardrobe.SkinCharacter.sharedMaterials[0].SetColor("_Albedo_Tint", wardrobe2.SkinTones[skinId]);
        wardrobe.SkinCharacter.sharedMaterials[1].SetColor("_Albedo_Tint", wardrobe2.SkinTones[skinId]);
        wardrobe.SkinCharacter.sharedMaterials[1].SetColor("_Mask1_Rchannel_ColorAmountA", Color.black);

        wardrobe.SkinCharacter.sharedMaterials[2].SetColor("_IrisBaseColor", eyesColor);
        wardrobe.SkinCharacter.sharedMaterials[2].SetColor("_IrisExtraColorAmount", eyesColor);

        wardrobe.HairMeshRenderer.sharedMaterial.SetColor("_BaseTint", hairColor);
        wardrobe.SkinCharacter.sharedMaterials[3].SetColor("_BaseColor", hairColor);

        if (!wardrobe.enemyData.isMale) {
            Color makeUpColor = wardrobe2.MakeupColors[makeUpId];
            Color lipsColor = wardrobe2.LipColors[lipsId];

            switch (makeUpIntense) {
                case 1:
                    makeUpColor.a = 0.5f;
                    lipsColor.a = 0.5f;
                    break;
                case 2:
                    makeUpColor.a = 0.7f;
                    lipsColor.a = 0.7f;
                    break;
                case 3:
                    makeUpColor.a = 0.9f;
                    lipsColor.a = 1.0f;
                    break;
                default:
                    makeUpColor.a = 0f;
                    lipsColor.a = 0f;
                    break;
            }

            wardrobe.SkinCharacter.sharedMaterials[1].SetColor("_Mask2_Rchannel_ColorAmountA", makeUpColor);
            wardrobe.SkinCharacter.sharedMaterials[0].SetColor("_Mask1_Rchannel_ColorAmountA", makeUpColor);
            wardrobe.SkinCharacter.sharedMaterials[1].SetColor("_Mask2_Bchannel_ColorAmountA", lipsColor);
            wardrobe.SkinCharacter.sharedMaterials[1].SetFloat("_GlossAdjust_Mask2Bchannel", 0.9f);

            Color.RGBToHSV(skinColor, out float skinH, out float skinS, out float skinV);

            float areolaH = skinH - RandomUtils.Float(0.1f, 0.5f);
            float areolaS = skinS + RandomUtils.Float(0.1f, 0.5f);
            float areolaV = skinV - RandomUtils.Float(0.1f, 0.5f);

            Color areolaColor = Color.HSVToRGB(areolaH, areolaS, areolaV);
            areolaColor.a = RandomUtils.Int32(60, 100) / 100f;
            wardrobe.SkinCharacter.sharedMaterials[0].SetColor("_Mask1_Bchannel_ColorAmountA", areolaColor);
        }

        if (wardrobe.enemyData.isMale || wardrobe2 is not null && wardrobe.SkinDick.sharedMesh != wardrobe2.StrapMesh) {
            wardrobe.SkinCharacter.SetBlendShapeWeight(0, 100);
            Material newdix = UnityEngine.Object.Instantiate(wardrobe.SkinDick.sharedMaterial);
            wardrobe.SkinDick.sharedMaterial = newdix;
            wardrobe.SkinDick.sharedMaterial.SetColor("_Albedo_Tint", skinColor);
        }

        CheckHat(wardrobe);
    }

    public static void SetEnemyDickType(Wardrobe wardrobe, CharacterSex characterSex) {
        if (characterSex.IsMale)
            return;

        var wardrobe2 = GameObject.Find("WardrobeOBJ")?.GetComponentWithCast<Wardrobe2>();
        if (wardrobe2 is null)
            return;

        if (RandomUtils.Chance(ChanceForFuta.Value)) {
            Plugin.Log.Info($"{characterSex.characterName} will use a dick");

            wardrobe.SkinDick.sharedMesh = RandomUtils.Chance(ChanceForFullFuta.Value) ? wardrobe2.DickMesh : wardrobe2.DickHalfMesh;
            Material material = characterSex.IsMale ? UnityEngine.Object.Instantiate(wardrobe2.DickMatM) : UnityEngine.Object.Instantiate(wardrobe2.DickMatF);
            wardrobe.SkinDick.sharedMaterial = material;
            var color = wardrobe.SkinCharacter.material.GetColor("_Albedo_Tint");
            wardrobe.SkinDick.sharedMaterial.SetColor("_Albedo_Tint", color);
        } else {
            Plugin.Log.Info($"{characterSex.characterName} will use strapon");

            wardrobe.SkinDick.sharedMesh = wardrobe2.StrapMesh;
            Material material = UnityEngine.Object.Instantiate(wardrobe2.StrapMat);
            wardrobe.SkinDick.sharedMaterial = material;
            var color = wardrobe.enemyData.customizationDATA.StrapOnColor;
            wardrobe.SkinDick.sharedMaterial.SetColor("_Albedo_Tint", color);
        }
    }

    public static void SetFutaState(CharacterSex characterSex, Wardrobe wardrobe) {
        if (!Enabled.Value)
            return;

        if (Character.adultSettingsDATA.EREnabled)
            return;

        var wardrobe2 = GameObject.Find("WardrobeOBJ")?.GetComponentWithCast<Wardrobe2>();
        if (wardrobe2 is null)
            return;

        Plugin.Log.Debug($"SetFutaState: {(wardrobe.SkinDick.sharedMesh.name != wardrobe2.StrapMesh.name ? "YES" : "No")}");

        if (wardrobe.SkinDick.sharedMesh != wardrobe2.StrapMesh)
            characterSex.IsFuta = true;
        else
            characterSex.IsFuta = false;
    }

    public static void CheckHat(Wardrobe wardrobe) {
        if (wardrobe.enemyData.customizationDATA.WearingHat) {
            Plugin.Log.Info("Hat");
            wardrobe.SetHairEnCreator(true);
            return;
        }
    }

    private static float GetSkewedValue(float max) {
        float u = RandomUtils.Float(0.0f, 1.0f);       // [0, 1]
        float skewed = u * u;                          // смещает значения к 0
        return skewed * max;
    }

    private static List<BodyProfile> GetBodyProfiles() { 
        var list = new List<BodyProfile> {
            new() {
                Name = "Slender",
                Muscle = 0.7f,
                Torso = 1.0f,
                Hips = 1.0f,
                Belly = 1.0f,
                Arms = 1.0f,
                Biceps = 1.0f,
                Thighs = 1.0f,
                Calves = 1.0f,
                Boobs = 0.9f,
                AreolaSize = 2,
                Booty = 1.0f,
                Dick = 0.9f
            },
            new() {
                Name = "Athletic",
                Muscle = 1.5f,
                Torso = 1.1f,
                Hips = 1.1f,
                Belly = 1.2f,
                Arms = 1.2f,
                Biceps = 1.2f,
                Thighs = 1.2f,
                Calves = 1.1f,
                Boobs = 1.1f,
                AreolaSize = 3,
                Booty = 1.1f,
                Dick = 1.2f
            },
            new() {
                Name = "Curvy",
                Muscle = 1.0f,
                Torso = 1.0f,
                Hips = 1.4f,
                Belly = 1.3f,
                Arms = 1.0f,
                Biceps = 1.0f,
                Thighs = 1.2f,
                Calves = 1.0f,
                Boobs = 1.3f,
                AreolaSize = 5,
                Booty = 1.4f,
                Dick = 1.0f
            },
            new() {
                Name = "Heavy",
                Muscle = 1.3f,
                Torso = 1.3f,
                Hips = 1.3f,
                Belly = 1.4f,
                Arms = 1.3f,
                Biceps = 1.3f,
                Thighs = 1.3f,
                Calves = 1.3f,
                Boobs = 1.0f,
                AreolaSize = 5,
                Booty = 1.2f,
                Dick = 1.1f
            },
            new() {
                Name = "Jacked",
                Muscle = 2.0f,
                Torso = 1.4f,
                Hips = 1.2f,
                Belly = 1.2f,
                Arms = 1.8f,
                Biceps = 1.8f,
                Thighs = 1.8f,
                Calves = 1.6f,
                Boobs = 1.0f,
                AreolaSize = 6,
                Booty = 1.0f,
                Dick = 1.3f
            },
            new() {
                Name = "Bimbo",
                Muscle = 0.5f,
                Torso = 0.8f,
                Hips = 1.3f,
                Belly = 0.9f,
                Arms = 0.7f,
                Biceps = 0.7f,
                Thighs = 1.2f,
                Calves = 1.0f,
                Boobs = 1.8f,
                AreolaSize = 7,
                Booty = 1.6f,
                Dick = 0.9f
            },
            new() {
                Name = "Sex Doll",
                Muscle = 1.0f,
                Torso = 1.0f,
                Hips = 1.0f,
                Belly = 1.0f,
                Arms = 1.0f,
                Biceps = 1.0f,
                Thighs = 1.0f,
                Calves = 1.0f,
                Boobs = 1.5f,
                AreolaSize = 8,
                Booty = 1.5f,
                Dick = 1.0f
            },
            new() {
                Name = "Futa",
                Muscle = 1.2f,
                Torso = 1.0f,
                Hips = 1.1f,
                Belly = 1.0f,
                Arms = 1.0f,
                Biceps = 1.0f,
                Thighs = 1.0f,
                Calves = 1.0f,
                Boobs = 1.4f,
                AreolaSize = 5,
                Booty = 1.3f,
                Dick = 1.3f
            },
            new() {
                Name = "Succubus",
                Muscle = 1.0f,
                Torso = 1.0f,
                Hips = 1.2f,
                Belly = 1.0f,
                Arms = 0.8f,
                Biceps = 0.8f,
                Thighs = 1.1f,
                Calves = 1.0f,
                Boobs = 1.6f,
                AreolaSize = 6,
                Booty = 1.5f,
                Dick = 1.1f
            },
            new() {
                Name = "Petite Lolita",
                Muscle = 0.4f,
                Torso = 0.7f,
                Hips = 0.8f,
                Belly = 0.8f,
                Arms = 0.6f,
                Biceps = 0.6f,
                Thighs = 0.7f,
                Calves = 0.7f,
                Boobs = 0.6f,
                AreolaSize = 6,
                Booty = 0.7f,
                Dick = 0.8f
            },
            new() {
                Name = "Amazon",
                Muscle = 2.0f,
                Torso = 1.3f,
                Hips = 1.2f,
                Belly = 1.1f,
                Arms = 1.8f,
                Biceps = 1.6f,
                Thighs = 1.8f,
                Calves = 1.6f,
                Boobs = 1.0f,
                AreolaSize = 1,
                Booty = 1.1f,
                Dick = 1.1f
            }
        };
        return list;
    }
    
    private static List<EnemyRace> GetEnemyRaces() {
        List<EnemyRace> enemyRaces = [
            new EnemyRace {
                Name = "European",
                SkinColors = ["#FFEFE0", "#FFDAB3","#FFC28C","#FFAC66","#E6954F"],
                HairColors = ["#1C0F00", "#2C1B0A", "#3A2F17", "#4B3821", "#5E4A2D", "#846644","#A67B5B", "#C19A6B", "#D2B48C", "#F1C27D", "#E0AC69", "#FFD700"],
                HairFantasyColors = ["#5F00FF", "#00FFFF", "#FF1493","#7FFF00", "#C0C0C0"],
                EyesColors = ["#2E1A47", "#1E3A5F", "#4B3621", "#A3A3A3", "#627A72", "#3A5F3E", "#7F462C"],
                EyesFantasyColors = ["#FFD700", "#00FF7F", "#8A2BE2", "#FF4500", "#FFFFFF"]
            },
            new EnemyRace {
                Name = "African",
                SkinColors = ["#4E342E", "#5D4037", "#6D4C41", "#795548", "#8D6E63"],
                HairColors = ["#0B0B0B", "#1A1A1A", "#2F2F2F", "#3D3D3D", "#4A4A4A", "#5A5A5A", "#6B6B6B", "#7C7C7C", "#8E8E8E", "#A0A0A0"],
                HairFantasyColors = ["#FF00FF", "#00FFBB", "#FFD300", "#FF5500", "#B19CD9"],
                EyesColors = ["#1B0B00","#3E2723","#5D4037","#795548","#6A1B9A","#004D40"],
                EyesFantasyColors = ["#00FFFF", "#FF1493", "#FFFF00", "#FFFFFF", "#8B0000"]
            },
            new EnemyRace {
                Name = "Latin",
                SkinColors          = ["#E7AC7B","#D28F5A","#BD6E3C","#A55A31","#8F3F2A"],
                HairColors          = ["#1C0A00","#2A1C0E","#3C2F1D","#5B3A26","#7A4E2F","#9C6733","#BF7A41","#D29969","#E5BA92","#F0D3B8","#2F150F"],
                HairFantasyColors   = ["#FF00AA","#00AAFF","#AAFF00","#FFAA00","#AA00FF"],
                EyesColors          = ["#3E2723","#5D4037","#6D4C41","#8E6B55","#556B2F","#336699","#7A5230"],
                EyesFantasyColors   = ["#FFD700", "#00CED1", "#FF4500", "#DA70D6", "#FFFFFF"]
            },
            new EnemyRace {
                Name = "Asian",
                SkinColors          = ["#FFE1C4","#FFD2A6","#E8B589","#C99467","#A67C52"],
                HairColors          = ["#000000","#1C1C1C","#333333","#4D4D4D","#666666","#7F7F7F","#999999","#B2B2B2","#CCCCCC","#E5E5E5"],
                HairFantasyColors   = ["#00FF7F","#FF69B4","#8A2BE2","#1E90FF","#FFD700"],
                EyesColors          = ["#3E2723","#5D4037","#8E6B55","#336699","#4B3621","#627A72"],
                EyesFantasyColors   = ["#00FFFF", "#FF1493", "#ADFF2F", "#B22222", "#FFFFFF"]
            },
        ];
        return enemyRaces;
    }
}
