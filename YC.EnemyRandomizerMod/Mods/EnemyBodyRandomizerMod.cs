using System;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;

using EnemyRandomizerMod.Models;

using UnityEngine;

namespace YC.EnemyRandomizerMod.Mods;
internal class EnemyBodyRandomizerMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<int> BodyHarmony;
    internal static ConfigEntry<int> ChanceForFuta;
    internal static ConfigEntry<int> ChanceForFullFuta;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    #endregion

    #region Storage
    internal static CharacterDataa Character => CharacterDataa.Instance;
    #endregion

    internal static void Load(ConfigFile config) {
        try {
            Enabled = config.Bind(nameof(EnemyBodyRandomizerMod), nameof(Enabled), false,
                new ConfigDescription("Activates the modification", new AcceptableValueList<bool>([true, false])));
            BodyHarmony = config.Bind(nameof(EnemyBodyRandomizerMod), nameof(BodyHarmony), 75,
                new ConfigDescription("Body size harmony value", new AcceptableValueRange<int>(0, 100)));
            ChanceForFuta = config.Bind(nameof(EnemyBodyRandomizerMod), nameof(ChanceForFuta), 35,
                new ConfigDescription("Chance for female character with active or mixed role become futanari", new AcceptableValueRange<int>(0, 100)));
            ChanceForFullFuta = config.Bind(nameof(EnemyBodyRandomizerMod), nameof(ChanceForFullFuta), 50,
                new ConfigDescription("Chance for female futa character get full futa (dick + balls)", new AcceptableValueRange<int>(0, 100)));

        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void Apply(EnemyWardrobe enemyWardrobe) {
        try {
            if (!Enabled.Value)
                return;

            //if (Character.adultSettingsDATA.EREnabled)
            //    return;

            if (!enemyWardrobe.gameObject.TryGetComponentWithCast(out CharacterSex characterSex))
                return;

            //SetHair(enemyWardrobe);
            SetFaceSize(enemyWardrobe, characterSex);
            SetBodySize(enemyWardrobe, characterSex);
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return;
        }
    }

    internal static void SetHair(EnemyWardrobe enemyWardrobe) {
        if (enemyWardrobe.gameObject.TryGetComponentWithCast(out AppereanceRandomizer appereanceRandomizer)) {
            var hairId = RandomUtils.Int32(appereanceRandomizer.Hairs.Count);
            foreach(var hairObj in appereanceRandomizer.Hairs)
                hairObj.SetActive(false);

            appereanceRandomizer.Hairs[hairId].SetActive(true);

            var hairColorId = RandomUtils.Int32(appereanceRandomizer.HairColors.Count);
            var hairMaterial = appereanceRandomizer.HairColors[hairColorId];
            hairMaterial.SetFloat("_AlphaClipThreshold", 0.0f);
            hairMaterial.SetFloat("_AnisotropyValue", RandomUtils.Float(0.5f, 0.95f));
        }
    }

    internal static void SetFaceSize(EnemyWardrobe enemyWardrobe, CharacterSex characterSex) {
        #region Face Size
        int face = RandomUtils.Int32(0, 20);
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

        if (characterSex.IsMale)
            enemyWardrobe.SkinCharacter.SetBlendShapeWeight(1, 100f);

        enemyWardrobe.SkinCharacter.materials[1].SetColor("_Mask1_Gchannel_ColorAmountA", new Color { r = 0.596f, g = 0f, b = 0.129f, a = GetSkewedValue(0.5f) });
    }

    internal static void SetBodySize(EnemyWardrobe enemyWardrobe, CharacterSex characterSex) {
        float bodyScale = Math.Clamp(RandomUtils.NormalFloat(0.55f, 0.12f), 0.0f, 1.0f);
        float muscle = Math.Clamp(RandomUtils.NormalFloat(0.6f, 0.25f), 0.0f, 1.0f);
        float bodyHarmony = BodyHarmony.Value / 100.0f;

        bodyScale = Lerp(bodyScale, bodyScale + RandomUtils.Float(-0.15f, 0.15f), 1.0f - bodyHarmony);
        muscle = Lerp(muscle, Math.Clamp(RandomUtils.NormalFloat(1.5f, 0.5f), 0.0f, 2.5f), 1.0f - bodyHarmony);


        var body = new CharacterBody {
            Muscle = Math.Clamp(muscle, 0.0f, 2.5f),
            Torso = Map( bodyScale + RandomUtils.Float(-0.05f, 0.1f), 0.0f, 1.0f, 0.3f, 2.5f ),
            Hips = Map( bodyScale + RandomUtils.Float(-0.05f, 0.05f), 0.0f, 1.0f, 0.3f, 3.5f ),
            Belly = Map( bodyScale + RandomUtils.Float(-0.1f, 0.2f), 0.0f, 1.0f, 0.3f, 4.0f ),
            Arms = Map( bodyScale * 0.7f + muscle * 0.12f + RandomUtils.Float(-0.05f, 0.15f), 0.3f, 1.5f, 0.3f, 5.0f ),
            Biceps = Map( muscle * 0.9f + RandomUtils.Float(-0.1f, 0.2f), 0.0f, 2.0f, 0.3f, 7.0f ),
            Thighs = Map( bodyScale * 0.5f + muscle * 0.3f + RandomUtils.Float(-0.05f, 0.15f), 0.0f, 1.8f, 0.3f, 4.5f ),
            Calves = Map( bodyScale * 0.4f + muscle * 0.25f + RandomUtils.Float(-0.1f, 0.2f), 0.0f, 1.6f, 0.3f, 4.5f ),
            Boobs = characterSex.IsMale
                ? 1.0f
                : Map(bodyScale + RandomUtils.Float(-0.1f, 0.1f), 0f, 1f, 0.7f, 1.5f),
            Booty = Map( bodyScale + RandomUtils.Float(-0.1f, 0.1f), 0.0f, 1.0f, 0.5f, 1.5f ),
            Dick = characterSex.IsMale
                ? Map( bodyScale + RandomUtils.Float(-0.05f, 0.1f), 0.0f, 1.0f, 0.7f, 1.5f )
                : Map( bodyScale + RandomUtils.Float(-0.1f, 0.1f), 0.0f, 1.0f, 0.7f, 1.5f )
        };

        Plugin.Log.Info($"Set body params for character {characterSex.characterName}. Muscle: {body.Muscle}, bodyScale: {bodyScale}");
        enemyWardrobe.SkinCharacter.materials[0].SetFloat("_FinalNormalMapPower", body.Muscle);
        float smoothness = RandomUtils.Float(0.0f, 0.9f);
        enemyWardrobe.SkinCharacter.material.SetFloat("_SmoothnessDeviate", smoothness);
        enemyWardrobe.SkinDick.material.SetFloat("_SmoothnessDeviate", smoothness);

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
            x = body.Booty,
            y = body.Booty,
            z = body.Booty
        };
        enemyWardrobe.Dick.transform.localScale = dick;

        EnemyDickType(enemyWardrobe, characterSex);
    }

    internal static void EnemyDickType(EnemyWardrobe enemyWardrobe, CharacterSex characterSex) {
        if (characterSex.IsMale)
            return;

        var wardrobe = GameObject.Find("WardrobeOBJ")?.GetComponentWithCast<EnemyWardrobe2>();
        if (wardrobe is null)
            return;

        if (RandomUtils.Chance(ChanceForFuta.Value)) {
            Plugin.Log.Info($"{characterSex.characterName} will use a dick");

            enemyWardrobe.SkinDick.sharedMesh = RandomUtils.Chance(ChanceForFullFuta.Value) ? wardrobe.DickMesh : wardrobe.DickHalfMesh;
            Material material = UnityEngine.Object.Instantiate(wardrobe.DickMatF);
            enemyWardrobe.SkinDick.material = material;
            var color = enemyWardrobe.SkinCharacter.material.GetColor("_Albedo_Tint");
            material.SetColor("_Albedo_Tint", color);
        } else {
            Plugin.Log.Info($"{characterSex.characterName} will use strapon");

            enemyWardrobe.SkinDick.sharedMesh = wardrobe.StrapMesh;
            Material material = UnityEngine.Object.Instantiate(wardrobe.StrapMat);
            enemyWardrobe.SkinDick.material = material;
            var color = enemyWardrobe.enemyData.customizationDATA.StrapOnColor;
            material.SetColor("_Albedo_Tint", color);
        }
    }

    private static float Map(float value, float inMin, float inMax, float outMin, float outMax) {
        return outMin + (Math.Clamp(value, inMin, inMax) - inMin) / (inMax - inMin) * (outMax - outMin);
    }

    private static float Lerp(float a, float b, float t) => a + (b - a) * t;

    private static float GetSkewedValue(float max) {
        float u = RandomUtils.Float(0.0f, 1.0f);       // [0, 1]
        float skewed = u * u;                          // смещает значения к 0
        return skewed * max;
    }
}