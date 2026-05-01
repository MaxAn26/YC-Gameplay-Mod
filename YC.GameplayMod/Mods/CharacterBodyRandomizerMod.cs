using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using YC.GameplayMod.Models;

namespace YC.GameplayMod.Mods;
public class CharacterBodyRandomizerMod
{
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> RandomizeCompanions;
    internal static MelonPreferences_Entry<int> ChanceForFuta;
    internal static MelonPreferences_Entry<int> ChanceForFullFuta;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static BodyRestrictions BodyRestrictions { get; private set; } = new();
    internal static List<BodyProfile> BodyProfiles { get; private set; } = [];
    internal static List<EnemyEthnicity> EnemyEthnicities { get; private set; } = [];
    #endregion

    #region Storage
    public static CharacterDataa Character => CharacterDataa.Instance;
    #endregion

    public static void Load(PluginConfig config)
    {
        try
        {
            Enabled = config.Entry(nameof(CharacterBodyRandomizerMod), nameof(Enabled), false,
                "Activates the modification", new PluginConfig.AcceptableValueList<bool>([true, false]));
            RandomizeCompanions = config.Entry(nameof(CharacterBodyRandomizerMod), nameof(RandomizeCompanions), false,
                "Randomize player companions", new PluginConfig.AcceptableValueList<bool>([true, false]));
            ChanceForFuta = config.Entry(nameof(CharacterBodyRandomizerMod), nameof(ChanceForFuta), 35,
                "Chance for female character with active or mixed role become futanari", new PluginConfig.AcceptableValueRange<int>(0, 100));
            ChanceForFullFuta = config.Entry(nameof(CharacterBodyRandomizerMod), nameof(ChanceForFullFuta), 50,
                "Chance for female futa character get full futa (dick + balls)", new PluginConfig.AcceptableValueRange<int>(0, 100));

            if (Enabled.Value)
            {
                if (!JsonUtils.TryDeserialize(GameplayMod.PluginResources, "BodyRestrictions.json", out BodyRestrictions bodyRestrictions))
                {
                    bodyRestrictions = GetBodyRestrictions();
                    JsonUtils.TrySerialize(GameplayMod.PluginResources, "BodyRestrictions.json", bodyRestrictions);
                }

                BodyRestrictions = bodyRestrictions;

                if (!JsonUtils.TryDeserialize(GameplayMod.PluginResources, "BodyProfileWeights.json", out List<BodyProfile> profiles))
                {
                    profiles = GetBodyProfiles();
                    JsonUtils.TrySerialize(GameplayMod.PluginResources, "BodyProfileWeights.json", profiles);
                }

                BodyProfiles.AddRange(profiles);

                if (!JsonUtils.TryDeserialize(GameplayMod.PluginResources, "EnemyEthnicities.json", out List<EnemyEthnicity> ethnicities))
                {
                    ethnicities = GetEnemyEthnicities();
                    JsonUtils.TrySerialize(GameplayMod.PluginResources, "EnemyEthnicities.json", ethnicities);
                }

                foreach (EnemyEthnicity ethnicity in ethnicities)
                {
                    BodyProfiles.ForEach(profile =>
                    {
                        if (ethnicity.BodyWeights.TryGetValue(profile.Name, out int weight))
                        {
                            ethnicity.BodyProfileWeights.Add(profile, weight);
                        }
                    });
                }

                EnemyEthnicities.AddRange(ethnicities);
            }

        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex.Message);
        }
    }

    public static void Randomize(CombatEnemyManager combatEnemyManager, CharacterSex characterSex)
    {
        try
        {
            if (!Enabled.Value)
            {
                return;
            }

            if (Character.adultSettingsDATA.EREnabled || characterSex.characterAttributes is null)
            {
                return;
            }

            Wardrobe wardrobe = characterSex.wardrobe;
            Wardrobe2 wardrobe2 = combatEnemyManager.wardrobe;
            if (wardrobe?.enemyData is null || wardrobe2 is null)
            {
                return;
            }

            if (!combatEnemyManager.requiredAllies.Contains(characterSex.characterName) || RandomizeCompanions.Value)
            {
                Material skin = UnityEngine.Object.Instantiate(wardrobe.SkinCharacter.sharedMaterials[0]);
                Material face = UnityEngine.Object.Instantiate(wardrobe.SkinCharacter.sharedMaterials[1]);
                Material eyes = UnityEngine.Object.Instantiate(wardrobe.SkinCharacter.sharedMaterials[2]);
                Material beard = UnityEngine.Object.Instantiate(wardrobe.SkinCharacter.sharedMaterials[3]);

                Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Material> materials = wardrobe.SkinCharacter.materials;
                materials[0] = skin;
                materials[1] = face;
                materials[2] = eyes;
                materials[3] = beard;
                wardrobe.SkinCharacter.materials = materials;

                EnemyEthnicity ethnicity = GetEnemyEthnicity(wardrobe.enemyData.statsDATA.EnemyEthnicity);
                BodyProfile profile = GetBodyProfile(ethnicity.BodyProfileWeights);
                CharacterBody body = CalculateBody(profile, characterSex.IsMale);

                GameplayMod.Log.Msg($"{characterSex.characterName}: Ethnicity: {ethnicity.Name}, Profile: {profile.Name}, {body}");

                #region Character skin
                Color skinColor = ethnicity.SkinTones.Count > 0
                    ? ethnicity.SkinTones.RandomItem().ToUnityColor()
                    : RandomUtils.Item([.. combatEnemyManager.SkinTones]);

                GameplayMod.Log.Msg($"{wardrobe.characterSex.characterName}: skin color: {skinColor}");

                wardrobe.SkinCharacter.sharedMaterials[0].SetColor("_Albedo_Tint", skinColor);
                #endregion Character skin

                #region Character hair
                /*if (wardrobe.HairMeshes.Count > 0) {
                    CheckHat(ref wardrobe);

                    var hairMesh = wardrobe.characterSex.IsMale
                        ? RandomUtils.Int32( 0, 15 )
                        : RandomUtils.Int32( 16, wardrobe.HairMeshes.Count - 1 );

                    if (wardrobe.HairMeshFilter.mesh != wardrobe.HatHair)
                        wardrobe.HairMeshFilter.mesh = wardrobe.HairMeshes[hairMesh];
                }
                Material hairMat = UnityEngine.Object.Instantiate(wardrobe.HairMat);
                wardrobe.HairMeshRenderer.sharedMaterial = hairMat;
                wardrobe.HairMeshRenderer.sharedMaterial.SetFloat("_AlphaClipThreshold", 0.0f);
                wardrobe.HairMeshRenderer.sharedMaterial.SetFloat("_AnisotropyValue", RandomUtils.Float(0.5f, 0.95f));

                Color hairColor = Color.black;
                if (ethnicity.HairColors.Count > 0) {
                    hairColor = ethnicity.HairColors.RandomItem().ToUnityColor();
                } else {
                    int hairId = RandomUtils.Chance(15)
                            ? RandomUtils.Int32(29, combatEnemyManager.HairColors.Count - 1)
                            : RandomUtils.Int32(28);

                    hairColor = combatEnemyManager.HairColors[hairId];
                }

                GameplayMod.Log.Msg($"{wardrobe.characterSex.characterName}: hair color: {hairColor}");

                wardrobe.HairMeshRenderer.sharedMaterial.SetColor("_Tip_Color", hairColor);
                wardrobe.SkinCharacter.sharedMaterials[3].SetColor("_BaseColor", hairColor); */   // beard color
                Color hairColor = Color.black;
                #endregion Character hair

                #region Character face
                Dictionary<int, float> faceStyle = GetFaceStyle(ethnicity.FaceStyle);
                foreach ((int index, float value) in faceStyle)
                {
                    wardrobe.SkinCharacter.SetBlendShapeWeight(index, value);
                }

                Dictionary<int, float> noseStyle = GetNoseStyle(ethnicity.NoseStyle);
                foreach ((int index, float value) in noseStyle)
                {
                    wardrobe.SkinCharacter.SetBlendShapeWeight(index, value);
                }

                Dictionary<int, float> browStyle = GetBrowStyle(ethnicity.BrowStyle);
                foreach ((int index, float value) in browStyle)
                {
                    wardrobe.SkinCharacter.SetBlendShapeWeight(index, value);
                }

                Dictionary<int, float> mouthStyle = GetMouthStyle(ethnicity.MouthStyle);
                foreach ((int index, float value) in mouthStyle)
                {
                    wardrobe.SkinCharacter.SetBlendShapeWeight(index, value);
                }

                Dictionary<int, float> mouthLength = GetMouthLength(ethnicity.MouthLength);
                foreach ((int index, float value) in mouthLength)
                {
                    wardrobe.SkinCharacter.SetBlendShapeWeight(index, value);
                }

                Dictionary<int, float> lipsForward = GetLipsForward(ethnicity.LipsForward);
                foreach ((int index, float value) in lipsForward)
                {
                    wardrobe.SkinCharacter.SetBlendShapeWeight(index, value);
                }

                Dictionary<int, float> lipsSize = GetLipsSize(ethnicity.LipsSize);
                foreach ((int index, float value) in lipsSize)
                {
                    wardrobe.SkinCharacter.SetBlendShapeWeight(index, value);
                }

                Dictionary<int, float> earsStype = GetEarsStyle(ethnicity.EarsStyle);
                foreach ((int index, float value) in earsStype)
                {
                    wardrobe.SkinCharacter.SetBlendShapeWeight(index, value);
                }

                wardrobe.SkinCharacter.sharedMaterials[1].SetColor("_Albedo_Tint", skinColor);
                wardrobe.SkinCharacter.sharedMaterials[1].SetColor("_Mask1_Gchannel_ColorAmountA", new Color { r = 0.596f, g = 0f, b = 0.129f, a = GetSkewedValue(0.5f) }); // freckless
                #endregion Character face

                #region Character eyes
                Dictionary<int, float> eyesStyle = GetEyesStyle(ethnicity.EyesStyle);
                foreach ((int index, float value) in eyesStyle)
                {
                    wardrobe.SkinCharacter.SetBlendShapeWeight(index, value);
                }

                Color eyesColor = Color.black;
                if (ethnicity.EyesColors.Count > 0)
                {
                    eyesColor = ethnicity.EyesColors.RandomItem().ToUnityColor();
                }
                else
                {
                    int eyesId = RandomUtils.Chance(15)
                        ? RandomUtils.Int32(21, combatEnemyManager.EyeColors.Count - 1)
                        : RandomUtils.Int32(20);

                    eyesColor = combatEnemyManager.EyeColors[eyesId];
                }

                GameplayMod.Log.Msg($"{wardrobe.characterSex.characterName}: eyes color: {eyesColor}");

                wardrobe.SkinCharacter.sharedMaterials[2].SetColor("_IrisBaseColor", eyesColor);
                wardrobe.SkinCharacter.sharedMaterials[2].SetColor("_IrisExtraColorAmount", eyesColor);
                #endregion Character eyes

                #region Character body
                wardrobe.SkinCharacter.materials[0].SetFloat("_FinalNormalMapPower", body.Muscle);
                float smoothness = RandomUtils.Float(0.0f, 0.9f);
                wardrobe.SkinCharacter.materials[0].SetFloat("_SmoothnessDeviate", smoothness);
                wardrobe.SkinCharacter.materials[1].SetFloat("_SmoothnessDeviate", smoothness);
                wardrobe.SkinDick.material.SetFloat("_SmoothnessDeviate", smoothness);

                if (!characterSex.IsMale)
                {
                    int ind = Math.Clamp(body.Areola, 0, wardrobe2.MakeupBodyTex.Count - 1);
                    wardrobe.SkinCharacter.materials[0].SetTexture("_MakeUpMask1_RGB", wardrobe2.MakeupBodyTex[ind]); // текстура сосков

                    Color.RGBToHSV(skinColor, out float skinH, out float skinS, out float skinV);

                    float areolaH = skinH - RandomUtils.Float(0.1f, 0.5f);
                    float areolaS = skinS + RandomUtils.Float(0.1f, 0.5f);
                    float areolaV = skinV - RandomUtils.Float(0.1f, 0.5f);

                    var areolaColor = Color.HSVToRGB(areolaH, areolaS, areolaV);
                    areolaColor.a = RandomUtils.Int32(60, 100) / 100f;
                    wardrobe.SkinCharacter.sharedMaterials[0].SetColor("_Mask1_Bchannel_ColorAmountA", areolaColor);
                }

                GameplayMod.Log.Msg($"{characterSex.characterName} set body data...");
                var back = new Vector3
                {
                    x = body.Torso,
                    y = body.Torso,
                    z = body.Torso
                };
                wardrobe.Back.transform.localScale = back;

                var waist = new Vector3
                {
                    x = body.Hips,
                    y = body.Hips,
                    z = body.Hips
                };
                wardrobe.Waist.transform.localScale = waist;

                var belly = new Vector3
                {
                    x = body.Belly,
                    y = body.Belly,
                    z = body.Belly
                };
                wardrobe.Belly.transform.localScale = belly;

                var arms = new Vector3
                {
                    x = body.Arms,
                    y = body.Arms,
                    z = body.Arms
                };
                wardrobe.LeftArm.transform.localScale = arms;
                wardrobe.RightArm.transform.localScale = arms;

                var biceps = new Vector3
                {
                    x = body.Biceps,
                    y = body.Biceps,
                    z = body.Biceps
                };
                wardrobe.LeftShoulder.transform.localScale = biceps;
                wardrobe.RightShoulder.transform.localScale = biceps;

                var thighs = new Vector3
                {
                    x = body.Thighs,
                    y = body.Thighs,
                    z = body.Thighs
                };
                wardrobe.LeftThigh.transform.localScale = thighs;
                wardrobe.RightThigh.transform.localScale = thighs;

                var calves = new Vector3
                {
                    x = body.Calves,
                    y = body.Calves,
                    z = body.Calves
                };
                wardrobe.LeftLeg.transform.localScale = calves;
                wardrobe.RightLeg.transform.localScale = calves;

                var boobs = new Vector3
                {
                    x = body.Boobs,
                    y = body.Boobs,
                    z = body.Boobs
                };
                wardrobe.LeftBoob.transform.localScale = boobs;
                wardrobe.RightBoob.transform.localScale = boobs;

                var booty = new Vector3
                {
                    x = body.Booty,
                    y = body.Booty,
                    z = body.Booty
                };
                wardrobe.LeftBooty.transform.localScale = booty;
                wardrobe.RightBooty.transform.localScale = booty;

                var dick = new Vector3
                {
                    x = body.Dick,
                    y = body.Dick,
                    z = body.Dick
                };
                wardrobe.Dick.transform.localScale = dick;
                characterSex.DickSize = body.Dick;

                GameplayMod.Log.Msg($"{characterSex.characterName} end set body");
                #endregion Character body

                #region Character make up
                if (!characterSex.IsMale)
                {
                    Color.RGBToHSV(skinColor, out float sh, out float ss, out float sv);
                    Color.RGBToHSV(hairColor, out float hh, out float hs, out float hv);
                    Color.RGBToHSV(eyesColor, out float eh, out float es, out float ev);

                    int skinTone = 2; // neutral tone
                    float score = 0f;
                    if (sh < 0.1f || sh > 0.9f)
                    {
                        score += 0.5f; // красноватый → warm
                    }

                    if (sh > 0.5f && sh < 0.75f)
                    {
                        score -= 0.5f; // синеватый → cool
                    }

                    // --- волосы ---
                    if (hv < 0.3f)
                    {
                        score -= 0.3f; // тёмные → чаще cool
                    }

                    if (hh > 0.05f && hh < 0.15f)
                    {
                        score += 0.3f; // рыжие/золотые → warm
                    }

                    // --- глаза ---
                    if (eh > 0.5f && eh < 0.7f)
                    {
                        score -= 0.3f; // синие
                    }

                    if (eh > 0.2f && eh < 0.4f)
                    {
                        score += 0.2f; // зелёные
                    }

                    if (score > 0.2f)
                    {
                        skinTone = 3;   // warm tone
                    }

                    if (score < -0.2f)
                    {
                        skinTone = 1;   // cool tone
                    }

                    float contrast = Mathf.Abs(sv - hv);
                    float intensity = Mathf.Lerp(0.3f, 1.0f, contrast);

                    // Eyeshadow
                    float h = (eh + 0.5f) % 1f;
                    if (skinTone == 3)
                    {
                        h += 0.05f;
                    }
                    else if (skinTone == 1)
                    {
                        h -= 0.05f;
                    }

                    float s = RandomUtils.Float(0.4f, 0.8f);
                    float v = RandomUtils.Float(0.5f, 0.9f);
                    var eyeshadowColor = Color.HSVToRGB(Mathf.Clamp01(h), Mathf.Clamp01(s * intensity), Mathf.Clamp01(v));

                    // eye liner
                    s *= 0.5f;
                    v *= 0.3f;
                    var eyelinerColor = Color.HSVToRGB(Mathf.Clamp01(h), Mathf.Clamp01(s), Mathf.Clamp01(v));

                    // lipstic
                    h = skinTone switch
                    {
                        3 => Mathf.Lerp(sh, 0.03f, 0.7f),
                        1 => Mathf.Lerp(sh, 0.97f, 0.7f),
                        _ => Mathf.Lerp(sh, 0.0f, 0.5f)
                    };
                    s = Mathf.Lerp(0.5f, 0.9f, 1f - ss);
                    v = Mathf.Lerp(0.6f, 0.9f, 1f - sv);

                    h += RandomUtils.Float(-0.02f, 0.02f);
                    s *= RandomUtils.Float(0.9f, 1.1f);
                    v *= RandomUtils.Float(0.9f, 1.1f);

                    // --- 5. Clamp ---
                    h = Mathf.Repeat(h, 1f);
                    s = Mathf.Clamp01(s);
                    v = Mathf.Clamp01(v);

                    var lipstic = Color.HSVToRGB(Mathf.Clamp01(h), Mathf.Clamp01(s * intensity), Mathf.Clamp01(v));

                    // nails
                    h += RandomUtils.Float(-0.05f, 0.05f);
                    h = Mathf.Repeat(h, 1f);

                    if (RandomUtils.Chance(0.5))
                    {
                        v *= 0.7f;
                    }

                    if (RandomUtils.Chance(0.3))
                    {
                        s *= 1.2f;
                    }

                    var nailColor = Color.HSVToRGB(Mathf.Clamp01(h), Mathf.Clamp01(s * intensity), Mathf.Clamp01(v));

                    wardrobe.SkinCharacter.sharedMaterials[1].SetTexture("_MakeUpMask1_RGB", wardrobe2.MakeupTex[RandomUtils.Int32(wardrobe2.MakeupTex.Count - 2)]);    // eye liner
                    wardrobe.SkinCharacter.sharedMaterials[1].SetTexture("_MakeUpMask2_RGB", wardrobe2.MakeupTex2[RandomUtils.Int32(wardrobe2.MakeupTex2.Count - 1)]);  // shadow

                    wardrobe.SkinCharacter.sharedMaterials[1].SetFloat("_GlossAdjust_Mask2Bchannel", RandomUtils.Float(0.0f, 1.0f));

                    wardrobe.SkinCharacter.sharedMaterials[0].SetColor("_Mask1_Rchannel_ColorAmountA", nailColor); // nails
                    wardrobe.SkinCharacter.sharedMaterials[0].SetColor("_Mask1_Gchannel_ColorAmountA", lipstic); // tatoo
                    wardrobe.SkinCharacter.sharedMaterials[1].SetColor("_Mask1_Rchannel_ColorAmountA", eyelinerColor); // eye liner
                    wardrobe.SkinCharacter.sharedMaterials[1].SetColor("_Mask2_Rchannel_ColorAmountA", eyeshadowColor); // eye shadows
                    wardrobe.SkinCharacter.sharedMaterials[1].SetColor("_Mask2_Bchannel_ColorAmountA", lipstic);    // lipstic
                }
                #endregion Character make up
            }

            #region Character genetals
            if (characterSex.IsMale)
            {
                wardrobe.SkinCharacter.SetBlendShapeWeight(1, 100f);

                wardrobe.SkinDick.sharedMesh = wardrobe2.DickMesh;
                Material material = UnityEngine.Object.Instantiate(wardrobe2.DickMatM);
                Color color = wardrobe.SkinCharacter.material.GetColor("_Albedo_Tint");
                wardrobe.SkinDick.sharedMaterial = material;
                wardrobe.SkinDick.sharedMaterial.SetColor("_Albedo_Tint", color);
            }
            else
            {
                if (RandomUtils.Chance(ChanceForFuta.Value))
                {
                    GameplayMod.Log.Msg($"{characterSex.characterName} will use a dick");

                    wardrobe.SkinDick.sharedMesh = RandomUtils.Chance(ChanceForFullFuta.Value) ? wardrobe2.DickMesh : wardrobe2.DickHalfMesh;
                    Material material = UnityEngine.Object.Instantiate(wardrobe2.DickMatF);
                    Color color = wardrobe.SkinCharacter.material.GetColor("_Albedo_Tint");
                    wardrobe.SkinDick.sharedMaterial = material;
                    wardrobe.SkinDick.sharedMaterial.SetColor("_Albedo_Tint", color);
                }
                else
                {
                    GameplayMod.Log.Msg($"{characterSex.characterName} will use strapon");

                    wardrobe.SkinDick.sharedMesh = wardrobe2.StrapMesh;
                    Material material = UnityEngine.Object.Instantiate(wardrobe2.StrapMat);
                    wardrobe.SkinDick.sharedMaterial = material;
                    Color color = wardrobe.enemyData.customizationDATA.TorsoIntColor;
                    wardrobe.SkinDick.sharedMaterial.SetColor("_Albedo_Tint", color);
                }
            }
            #endregion Character genetals
        }
        catch (Exception ex)
        {
            GameplayMod.Log.Error(ex.Message);
        }
    }

    private static Dictionary<int, float> GetFaceStyle(List<int> faces)
    {
        int id = faces.Count > 0 ? faces.RandomItem() : RandomUtils.Int32(0, 20);
        Dictionary<int, float> values = [];
        switch (id)
        {
            case 1:
                values.Clear();
                values.Add(2, 35.0f);
                values.Add(3, 0.0f);
                values.Add(4, 0.0f);
                values.Add(5, 0.0f);
                break;
            case 2:
                values.Clear();
                values.Add(2, 70.0f);
                values.Add(3, 0.0f);
                values.Add(4, 0.0f);
                values.Add(5, 0.0f);
                break;
            case 3:
                values.Clear();
                values.Add(2, 100.0f);
                values.Add(3, 0.0f);
                values.Add(4, 0.0f);
                values.Add(5, 0.0f);
                break;
            case 4:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 35.0f);
                values.Add(4, 0.0f);
                values.Add(5, 0.0f);
                break;
            case 5:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 70.0f);
                values.Add(4, 0.0f);
                values.Add(5, 0.0f);
                break;
            case 6:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 100.0f);
                values.Add(4, 0.0f);
                values.Add(5, 0.0f);
                break;
            case 7:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 0.0f);
                values.Add(4, 35.0f);
                values.Add(5, 0.0f);
                break;
            case 8:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 0.0f);
                values.Add(4, 70.0f);
                values.Add(5, 0.0f);
                break;
            case 9:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 0.0f);
                values.Add(4, 100.0f);
                values.Add(5, 0.0f);
                break;
            case 10:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 0.0f);
                values.Add(4, 0.0f);
                values.Add(5, 35.0f);
                break;
            case 11:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 0.0f);
                values.Add(4, 0.0f);
                values.Add(5, 70.0f);
                break;
            case 12:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 0.0f);
                values.Add(4, 0.0f);
                values.Add(5, 100.0f);
                break;
            case 13:
                values.Clear();
                values.Add(2, 35.0f);
                values.Add(3, 35.0f);
                values.Add(4, 0.0f);
                values.Add(5, 0.0f);
                break;
            case 14:
                values.Clear();
                values.Add(2, 50.0f);
                values.Add(3, 50.0f);
                values.Add(4, 0.0f);
                values.Add(5, 0.0f);
                break;
            case 15:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 35.0f);
                values.Add(4, 35.0f);
                values.Add(5, 0.0f);
                break;
            case 16:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 50.0f);
                values.Add(4, 50.0f);
                values.Add(5, 0.0f);
                break;
            case 17:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 0.0f);
                values.Add(4, 35.0f);
                values.Add(5, 35.0f);
                break;
            case 18:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 0.0f);
                values.Add(4, 50.0f);
                values.Add(5, 50.0f);
                break;
            case 19:
                values.Clear();
                values.Add(2, 35.0f);
                values.Add(3, 0.0f);
                values.Add(4, 0.0f);
                values.Add(5, 35.0f);
                break;
            case 20:
                values.Clear();
                values.Add(2, 50.0f);
                values.Add(3, 0.0f);
                values.Add(4, 0.0f);
                values.Add(5, 50.0f);
                break;
            default:
                values.Clear();
                values.Add(2, 0.0f);
                values.Add(3, 0.0f);
                values.Add(4, 0.0f);
                values.Add(5, 0.0f);
                break;
        }

        return values;
    }

    private static Dictionary<int, float> GetEyesStyle(List<int> eyes)
    {
        int id = eyes.Count > 0 ? eyes.RandomItem() : RandomUtils.Int32(0, 20);
        Dictionary<int, float> values = [];
        switch (id)
        {
            case 1:
                values.Add(6, 35.0f);
                values.Add(7, 0.0f);
                values.Add(8, 0.0f);
                values.Add(9, 0.0f);
                break;
            case 2:
                values.Add(6, 70.0f);
                values.Add(7, 0.0f);
                values.Add(8, 0.0f);
                values.Add(9, 0.0f);
                break;
            case 3:
                values.Add(6, 100.0f);
                values.Add(7, 0.0f);
                values.Add(8, 0.0f);
                values.Add(9, 0.0f);
                break;
            case 4:
                values.Add(6, 0.0f);
                values.Add(7, 35.0f);
                values.Add(8, 0.0f);
                values.Add(9, 0.0f);
                break;
            case 5:
                values.Add(6, 0.0f);
                values.Add(7, 70.0f);
                values.Add(8, 0.0f);
                values.Add(9, 0.0f);
                break;
            case 6:
                values.Add(6, 0.0f);
                values.Add(7, 100.0f);
                values.Add(8, 0.0f);
                values.Add(9, 0.0f);
                break;
            case 7:
                values.Add(6, 0.0f);
                values.Add(7, 0.0f);
                values.Add(8, 35.0f);
                values.Add(9, 0.0f);
                break;
            case 8:
                values.Add(6, 0.0f);
                values.Add(7, 0.0f);
                values.Add(8, 70.0f);
                values.Add(9, 0.0f);
                break;
            case 9:
                values.Add(6, 0.0f);
                values.Add(7, 0.0f);
                values.Add(8, 100.0f);
                values.Add(9, 0.0f);
                break;
            case 10:
                values.Add(6, 0.0f);
                values.Add(7, 0.0f);
                values.Add(8, 0.0f);
                values.Add(9, 35.0f);
                break;
            case 11:
                values.Add(6, 0.0f);
                values.Add(7, 0.0f);
                values.Add(8, 0.0f);
                values.Add(9, 70.0f);
                break;
            case 12:
                values.Add(6, 0.0f);
                values.Add(7, 0.0f);
                values.Add(8, 0.0f);
                values.Add(9, 100.0f);
                break;
            case 13:
                values.Add(6, 35.0f);
                values.Add(7, 35.0f);
                values.Add(8, 0.0f);
                values.Add(9, 0.0f);
                break;
            case 14:
                values.Add(6, 50.0f);
                values.Add(7, 50.0f);
                values.Add(8, 0.0f);
                values.Add(9, 0.0f);
                break;
            case 15:
                values.Add(6, 0.0f);
                values.Add(7, 35.0f);
                values.Add(8, 35.0f);
                values.Add(9, 0.0f);
                break;
            case 16:
                values.Add(6, 0.0f);
                values.Add(7, 50.0f);
                values.Add(8, 50.0f);
                values.Add(9, 0.0f);
                break;
            case 17:
                values.Add(6, 0.0f);
                values.Add(7, 0.0f);
                values.Add(8, 35.0f);
                values.Add(9, 35.0f);
                break;
            case 18:
                values.Add(6, 0.0f);
                values.Add(7, 0.0f);
                values.Add(8, 50.0f);
                values.Add(9, 50.0f);
                break;
            case 19:
                values.Add(6, 35.0f);
                values.Add(7, 0.0f);
                values.Add(8, 0.0f);
                values.Add(9, 35.0f);
                break;
            case 20:
                values.Add(6, 50.0f);
                values.Add(7, 0.0f);
                values.Add(8, 0.0f);
                values.Add(9, 50.0f);
                break;
            default:
                values.Add(6, 0.0f);
                values.Add(7, 0.0f);
                values.Add(8, 0.0f);
                values.Add(9, 0.0f);
                break;
        }

        return values;
    }

    private static Dictionary<int, float> GetNoseStyle(List<int> noses)
    {
        int id = noses.Count > 0 ? noses.RandomItem() : RandomUtils.Int32(0, 12);
        Dictionary<int, float> values = [];
        switch (id)
        {
            case 1:
                values.Add(11, 35.0f);
                values.Add(12, 0.0f);
                values.Add(13, 0.0f);
                values.Add(14, 0.0f);
                break;
            case 2:
                values.Add(11, 70.0f);
                values.Add(12, 0.0f);
                values.Add(13, 0.0f);
                values.Add(14, 0.0f);
                break;
            case 3:
                values.Add(11, 100.0f);
                values.Add(12, 0.0f);
                values.Add(13, 0.0f);
                values.Add(14, 0.0f);
                break;
            case 4:
                values.Add(11, 0.0f);
                values.Add(12, 35.0f);
                values.Add(13, 0.0f);
                values.Add(14, 0.0f);
                break;
            case 5:
                values.Add(11, 0.0f);
                values.Add(12, 70.0f);
                values.Add(13, 0.0f);
                values.Add(14, 0.0f);
                break;
            case 6:
                values.Add(11, 0.0f);
                values.Add(12, 100.0f);
                values.Add(13, 0.0f);
                values.Add(14, 0.0f);
                break;
            case 7:
                values.Add(11, 0.0f);
                values.Add(12, 0.0f);
                values.Add(13, 35.0f);
                values.Add(14, 0.0f);
                break;
            case 8:
                values.Add(11, 0.0f);
                values.Add(12, 0.0f);
                values.Add(13, 70.0f);
                values.Add(14, 0.0f);
                break;
            case 9:
                values.Add(11, 0.0f);
                values.Add(12, 0.0f);
                values.Add(13, 100.0f);
                values.Add(14, 0.0f);
                break;
            case 10:
                values.Add(11, 0.0f);
                values.Add(12, 0.0f);
                values.Add(13, 0.0f);
                values.Add(14, 35.0f);
                break;
            case 11:
                values.Add(11, 0.0f);
                values.Add(12, 0.0f);
                values.Add(13, 0.0f);
                values.Add(14, 70.0f);
                break;
            case 12:
                values.Add(11, 0.0f);
                values.Add(12, 0.0f);
                values.Add(13, 0.0f);
                values.Add(14, 100.0f);
                break;
            default:
                values.Add(11, 0.0f);
                values.Add(12, 0.0f);
                values.Add(13, 0.0f);
                values.Add(14, 0.0f);
                break;
        }
        return values;
    }

    private static Dictionary<int, float> GetBrowStyle(List<int> brows)
    {
        int id = brows.Count > 0 ? brows.RandomItem() : RandomUtils.Int32(0, 8);
        Dictionary<int, float> values = [];
        switch (id)
        {
            case 1:
                values.Add(24, 50.0f);
                values.Add(25, 0.0f);
                values.Add(26, 0.0f);
                values.Add(27, 0.0f);
                break;
            case 2:
                values.Add(24, 100.0f);
                values.Add(25, 0.0f);
                values.Add(26, 0.0f);
                values.Add(27, 0.0f);
                break;
            case 3:
                values.Add(24, 0.0f);
                values.Add(25, 50.0f);
                values.Add(26, 0.0f);
                values.Add(27, 0.0f);
                break;
            case 4:
                values.Add(24, 0.0f);
                values.Add(25, 100.0f);
                values.Add(26, 0.0f);
                values.Add(27, 0.0f);
                break;
            case 5:
                values.Add(24, 0.0f);
                values.Add(25, 0.0f);
                values.Add(26, 50.0f);
                values.Add(27, 0.0f);
                break;
            case 6:
                values.Add(24, 0.0f);
                values.Add(25, 0.0f);
                values.Add(26, 100.0f);
                values.Add(27, 0.0f);
                break;
            case 7:
                values.Add(24, 0.0f);
                values.Add(25, 0.0f);
                values.Add(26, 0.0f);
                values.Add(27, 50.0f);
                break;
            case 8:
                values.Add(24, 0.0f);
                values.Add(25, 0.0f);
                values.Add(26, 0.0f);
                values.Add(27, 100.0f);
                break;
            default:
                values.Add(24, 0.0f);
                values.Add(25, 0.0f);
                values.Add(26, 0.0f);
                values.Add(27, 0.0f);
                break;
        }
        return values;
    }

    private static Dictionary<int, float> GetMouthStyle(List<int> mouth)
    {
        int id = mouth.Count > 0 ? mouth.RandomItem() : RandomUtils.Int32(0, 20);
        Dictionary<int, float> values = [];
        switch (id)
        {
            case 1:
                values.Add(16, 35.0f);
                values.Add(17, 0.0f);
                values.Add(18, 0.0f);
                values.Add(19, 0.0f);
                break;
            case 2:
                values.Add(16, 70.0f);
                values.Add(17, 0.0f);
                values.Add(18, 0.0f);
                values.Add(19, 0.0f);
                break;
            case 3:
                values.Add(16, 100.0f);
                values.Add(17, 0.0f);
                values.Add(18, 0.0f);
                values.Add(19, 0.0f);
                break;
            case 4:
                values.Add(16, 0.0f);
                values.Add(17, 35.0f);
                values.Add(18, 0.0f);
                values.Add(19, 0.0f);
                break;
            case 5:
                values.Add(16, 0.0f);
                values.Add(17, 70.0f);
                values.Add(18, 0.0f);
                values.Add(19, 0.0f);
                break;
            case 6:
                values.Add(16, 0.0f);
                values.Add(17, 100.0f);
                values.Add(18, 0.0f);
                values.Add(19, 0.0f);
                break;
            case 7:
                values.Add(16, 0.0f);
                values.Add(17, 0.0f);
                values.Add(18, 35.0f);
                values.Add(19, 0.0f);
                break;
            case 8:
                values.Add(16, 0.0f);
                values.Add(17, 0.0f);
                values.Add(18, 70.0f);
                values.Add(19, 0.0f);
                break;
            case 9:
                values.Add(16, 0.0f);
                values.Add(17, 0.0f);
                values.Add(18, 100.0f);
                values.Add(19, 0.0f);
                break;
            case 10:
                values.Add(16, 0.0f);
                values.Add(17, 0.0f);
                values.Add(18, 0.0f);
                values.Add(19, 35.0f);
                break;
            case 11:
                values.Add(16, 0.0f);
                values.Add(17, 0.0f);
                values.Add(18, 0.0f);
                values.Add(19, 70.0f);
                break;
            case 12:
                values.Add(16, 0.0f);
                values.Add(17, 0.0f);
                values.Add(18, 0.0f);
                values.Add(19, 100.0f);
                break;
            case 13:
                values.Add(16, 35.0f);
                values.Add(17, 35.0f);
                values.Add(18, 0.0f);
                values.Add(19, 0.0f);
                break;
            case 14:
                values.Add(16, 50.0f);
                values.Add(17, 50.0f);
                values.Add(18, 0.0f);
                values.Add(19, 0.0f);
                break;
            case 15:
                values.Add(16, 0.0f);
                values.Add(17, 35.0f);
                values.Add(18, 35.0f);
                values.Add(19, 0.0f);
                break;
            case 16:
                values.Add(16, 0.0f);
                values.Add(17, 50.0f);
                values.Add(18, 50.0f);
                values.Add(19, 0.0f);
                break;
            case 17:
                values.Add(16, 0.0f);
                values.Add(17, 0.0f);
                values.Add(18, 35.0f);
                values.Add(19, 35.0f);
                break;
            case 18:
                values.Add(16, 0.0f);
                values.Add(17, 0.0f);
                values.Add(18, 50.0f);
                values.Add(19, 50.0f);
                break;
            case 19:
                values.Add(16, 35.0f);
                values.Add(17, 0.0f);
                values.Add(18, 0.0f);
                values.Add(19, 35.0f);
                break;
            case 20:
                values.Add(16, 50.0f);
                values.Add(17, 0.0f);
                values.Add(18, 0.0f);
                values.Add(19, 50.0f);
                break;
            default:
                values.Add(16, 0.0f);
                values.Add(17, 0.0f);
                values.Add(18, 0.0f);
                values.Add(19, 0.0f);
                break;
        }
        return values;
    }

    private static Dictionary<int, float> GetMouthLength(List<int> mouth)
    {
        int id = mouth.Count > 0 ? mouth.RandomItem() : RandomUtils.Int32(0, 10);
        Dictionary<int, float> values = [];
        switch (id)
        {
            case 1:
                values.Add(20, 0.0f);
                values.Add(21, 20.0f);
                break;
            case 2:
                values.Add(20, 0.0f);
                values.Add(21, 40.0f);
                break;
            case 3:
                values.Add(20, 0.0f);
                values.Add(21, 60.0f);
                break;
            case 4:
                values.Add(20, 0.0f);
                values.Add(21, 80.0f);
                break;
            case 5:
                values.Add(20, 0.0f);
                values.Add(21, 100.0f);
                break;
            case 6:
                values.Add(20, 20.0f);
                values.Add(21, 0.0f);
                break;
            case 7:
                values.Add(20, 40.0f);
                values.Add(21, 0.0f);
                break;
            case 8:
                values.Add(20, 60.0f);
                values.Add(21, 0.0f);
                break;
            case 9:
                values.Add(20, 80.0f);
                values.Add(21, 0.0f);
                break;
            case 10:
                values.Add(20, 100.0f);
                values.Add(21, 0.0f);
                break;
            default:
                values.Add(20, 0.0f);
                values.Add(21, 0.0f);
                break;
        }
        return values;
    }

    private static Dictionary<int, float> GetLipsForward(List<int> lips)
    {
        int id = lips.Count > 0 ? lips.RandomItem() : RandomUtils.Int32(0, 10);
        Dictionary<int, float> values = [];
        switch (id)
        {
            case 1:
                values.Add(35, 10.0f);
                break;
            case 2:
                values.Add(35, 20.0f);
                break;
            case 3:
                values.Add(35, 30.0f);
                break;
            case 4:
                values.Add(35, 40.0f);
                break;
            case 5:
                values.Add(35, 50.0f);
                break;
            case 6:
                values.Add(35, 60.0f);
                break;
            case 7:
                values.Add(35, 70.0f);
                break;
            case 8:
                values.Add(35, 80.0f);
                break;
            case 9:
                values.Add(35, 90.0f);
                break;
            case 10:
                values.Add(35, 100.0f);
                break;
            default:
                values.Add(35, 0.0f);
                break;
        }
        return values;
    }

    private static Dictionary<int, float> GetLipsSize(List<int> lips)
    {
        int id = lips.Count > 0 ? lips.RandomItem() : RandomUtils.Int32(0, 8);
        Dictionary<int, float> values = [];
        switch (id)
        {
            case 1:
                values.Add(22, 12.0f);
                values.Add(23, 12.0f);
                break;
            case 2:
                values.Add(22, 24.0f);
                values.Add(23, 24.0f);
                break;
            case 3:
                values.Add(22, 36.0f);
                values.Add(23, 36.0f);
                break;
            case 4:
                values.Add(22, 48.0f);
                values.Add(23, 48.0f);
                break;
            case 5:
                values.Add(22, 54.0f);
                values.Add(23, 54.0f);
                break;
            case 6:
                values.Add(22, 66.0f);
                values.Add(23, 66.0f);
                break;
            case 7:
                values.Add(22, 88.0f);
                values.Add(23, 88.0f);
                break;
            case 8:
                values.Add(22, 100.0f);
                values.Add(23, 100.0f);
                break;
            default:
                values.Add(22, 0.0f);
                values.Add(23, 0.0f);
                break;
        }
        return values;
    }

    private static Dictionary<int, float> GetEarsStyle(List<int> ears)
    {
        int id = ears.Count > 0 ? ears.RandomItem() : RandomUtils.Int32(0, 20);
        Dictionary<int, float> values = [];
        switch (id)
        {
            case 1:
                values.Add(28, 20.0f);
                values.Add(29, 0.0f);
                break;
            case 2:
                values.Add(28, 40.0f);
                values.Add(29, 0.0f);
                break;
            case 3:
                values.Add(28, 60.0f);
                values.Add(29, 0.0f);
                break;
            case 4:
                values.Add(28, 80.0f);
                values.Add(29, 0.0f);
                break;
            case 5:
                values.Add(28, 100.0f);
                values.Add(29, 0.0f);
                break;
            case 6:
                values.Add(28, 0.0f);
                values.Add(29, 20.0f);
                break;
            case 7:
                values.Add(28, 0.0f);
                values.Add(29, 40.0f);
                break;
            case 8:
                values.Add(28, 0.0f);
                values.Add(29, 60.0f);
                break;
            case 9:
                values.Add(28, 0.0f);
                values.Add(29, 80.0f);
                break;
            case 10:
                values.Add(28, 0.0f);
                values.Add(29, 100.0f);
                break;
            default:
                values.Add(28, 0.0f);
                values.Add(29, 0.0f);
                break;
        }
        return values;
    }

    public static void SetFutaState(CharacterSex characterSex)
    {
        if (!Enabled.Value)
        {
            return;
        }

        if (Character.adultSettingsDATA.EREnabled)
        {
            return;
        }

        if (characterSex.IsMale)
        {
            return;
        }

        Wardrobe2 wardrobe2 = GameObject.Find("WardrobeOBJ")?.GetComponentWithCast<Wardrobe2>();
        if (wardrobe2 is null)
        {
            return;
        }

        GameplayMod.Log.Msg($"{characterSex.characterName}: SetFutaState: {(characterSex.wardrobe.SkinDick.sharedMesh != wardrobe2.StrapMesh ? "YES" : "No")}");

        characterSex.IsFuta = characterSex.wardrobe.SkinDick.sharedMesh != wardrobe2.StrapMesh;
    }

    public static void CheckHat(ref Wardrobe wardrobe)
    {
        if (wardrobe.enemyData.customizationDATA.WearingHat)
        {
            GameplayMod.Log.Msg("Hat");
            wardrobe.SetHairEnCreator(true);
            return;
        }
    }

    private static CharacterBody CalculateBody(BodyProfile bodyProfile, bool isMale)
    {
        float extraBoobs = 0.0f;
        if (!isMale && RandomUtils.Chance(25))
        {
            GameplayMod.Log.Msg("Extra boobs");
            extraBoobs += 0.25f;
        }

        float extraBooty = 0f;
        if (!isMale && RandomUtils.Chance(25))
        {
            GameplayMod.Log.Msg("Extra booty");
            extraBooty += 0.25f;
        }

        CharacterBody cb = new()
        {
            Areola = Math.Clamp(bodyProfile.Areola.GetSize(), BodyRestrictions.Areola.Min, BodyRestrictions.Areola.Max),
            Arms = Math.Clamp(bodyProfile.Arms.GetSize(), BodyRestrictions.Arms.Min, BodyRestrictions.Arms.Max),
            Belly = Math.Clamp(bodyProfile.Belly.GetSize(), BodyRestrictions.Belly.Min, BodyRestrictions.Belly.Max),
            Biceps = Math.Clamp(bodyProfile.Biceps.GetSize(), BodyRestrictions.Biceps.Min, BodyRestrictions.Biceps.Max),
            Boobs = Math.Clamp(bodyProfile.Boobs.GetSize(extraBoobs), BodyRestrictions.Boobs.Min, BodyRestrictions.Boobs.Max),
            Booty = Math.Clamp(bodyProfile.Booty.GetSize(extraBooty), BodyRestrictions.Booty.Min, BodyRestrictions.Booty.Max),
            Calves = Math.Clamp(bodyProfile.Calves.GetSize(), BodyRestrictions.Calves.Min, BodyRestrictions.Calves.Max),
            Dick = Math.Clamp(bodyProfile.Dick.GetSize(), BodyRestrictions.Dick.Min, BodyRestrictions.Dick.Max),
            Hips = Math.Clamp(bodyProfile.Hips.GetSize(), BodyRestrictions.Hips.Min, BodyRestrictions.Hips.Max),
            Muscle = Math.Clamp(bodyProfile.Muscle.GetSize(), BodyRestrictions.Muscle.Min, BodyRestrictions.Muscle.Max),
            Thighs = Math.Clamp(bodyProfile.Thighs.GetSize(), BodyRestrictions.Thighs.Min, BodyRestrictions.Thighs.Max),
            Torso = Math.Clamp(bodyProfile.Torso.GetSize(), BodyRestrictions.Torso.Min, BodyRestrictions.Torso.Max),
        };

        float fat = Normalize(cb.Belly, 0.3f, 4.0f);
        float muscle = Normalize(cb.Muscle, 0.0f, 2.5f);

        // --- 1. Жир распределяется по телу ---
        cb.Arms += fat * 0.3f;
        cb.Booty += fat * 0.3f;
        cb.Calves += fat * 0.3f;
        cb.Hips += fat * 0.4f;
        cb.Thighs += fat * 0.5f;

        // --- 2. Мышцы влияют на тело ---
        cb.Arms += muscle * 0.6f;
        cb.Biceps += muscle * 1.2f;
        cb.Calves += muscle * 0.5f;
        cb.Thighs += muscle * 0.6f;
        cb.Torso += muscle * 0.7f;

        // --- 3. Баланс верх/низ ---
        float lower = (cb.Thighs + cb.Calves) * 0.5f;
        float upper = (cb.Arms + cb.Biceps) * 0.5f;
        float diff = upper - lower;

        cb.Thighs -= diff * 0.3f;
        cb.Calves -= diff * 0.2f;

        // --- 4. Связка бедра/ягодицы ---
        cb.Booty += (cb.Hips - 1.0f) * 0.5f;
        cb.Thighs += (cb.Hips - 1.0f) * 0.4f;

        // --- 5. Торс ↔ живот ---
        cb.Torso += (cb.Belly - 1.0f) * 0.3f;

        // --- 6. Грудь ↔ жир ---
        cb.Boobs += fat * 0.4f;

        // --- 7. Ареолы ↔ грудь ---
        float areolaSize = cb.Areola;
        areolaSize += (cb.Boobs - 1.0f) * 2.0f;
        areolaSize = Mathf.Clamp(areolaSize, 0.0f, 7.0f);
        cb.Areola = Mathf.RoundToInt(areolaSize);

        // --- 8. Лёгкая корреляция размера тела ---
        cb.Dick += (cb.Torso - 1.0f) * 0.05f;

        // --- 9. Общая масса тела ---
        float mass = (cb.Belly + cb.Thighs + cb.Hips) / 3.0f;
        float scale = mass - 1.0f;

        cb.Arms += scale * 0.2f;
        cb.Calves += scale * 0.2f;
        cb.Torso += scale * 0.3f;

        // --- 10. Анти-экстрим ---
        if (cb.Belly > 3.0f)
        {
            cb.Muscle *= 0.8f;
        }

        if (cb.Muscle > 2.0f)
        {
            cb.Belly *= 0.85f;
        }

        cb.Areola = Mathf.Clamp(cb.Areola, BodyRestrictions.Areola.Min, BodyRestrictions.Areola.Max);
        cb.Arms = Math.Clamp(cb.Arms, BodyRestrictions.Arms.Min, BodyRestrictions.Arms.Max);
        cb.Belly = Math.Clamp(cb.Belly, BodyRestrictions.Belly.Min, BodyRestrictions.Belly.Max);
        cb.Biceps = Math.Clamp(cb.Biceps, BodyRestrictions.Biceps.Min, BodyRestrictions.Biceps.Max);
        cb.Boobs = Math.Clamp(cb.Boobs, BodyRestrictions.Boobs.Min, BodyRestrictions.Boobs.Max);
        cb.Booty = Math.Clamp(cb.Booty, BodyRestrictions.Booty.Min, BodyRestrictions.Booty.Max);
        cb.Calves = Math.Clamp(cb.Calves, BodyRestrictions.Calves.Min, BodyRestrictions.Calves.Max);
        cb.Dick = Math.Clamp(cb.Dick, BodyRestrictions.Dick.Min, BodyRestrictions.Dick.Max);
        cb.Hips = Math.Clamp(cb.Hips, BodyRestrictions.Hips.Min, BodyRestrictions.Hips.Max);
        cb.Muscle = Math.Clamp(cb.Muscle, BodyRestrictions.Muscle.Min, BodyRestrictions.Muscle.Max);
        cb.Thighs = Math.Clamp(cb.Thighs, BodyRestrictions.Thighs.Min, BodyRestrictions.Thighs.Max);
        cb.Torso = Math.Clamp(cb.Torso, BodyRestrictions.Torso.Min, BodyRestrictions.Torso.Max);

        return cb;

        static float Normalize(float value, float min, float max) => (value - min) / (max - min);
    }

    private static float GetSkewedValue(float max)
    {
        float u = RandomUtils.Float(0.0f, 1.0f);       // [0, 1]
        float skewed = u * u;                          // смещает значения к 0
        return skewed * max;
    }

    private static BodyRestrictions GetBodyRestrictions()
    {
        var restrictions = new BodyRestrictions
        {
            Areola = new BodyRestrictions.ValueRestrictions<int> { Min = 0, Max = 7 },
            Arms = new BodyRestrictions.ValueRestrictions<float> { Min = 0.3f, Max = 5.0f },
            Belly = new BodyRestrictions.ValueRestrictions<float> { Min = 0.3f, Max = 4.0f },
            Biceps = new BodyRestrictions.ValueRestrictions<float> { Min = 0.3f, Max = 7.0f },
            Boobs = new BodyRestrictions.ValueRestrictions<float> { Min = 0.7f, Max = 1.5f },
            Booty = new BodyRestrictions.ValueRestrictions<float> { Min = 0.5f, Max = 1.5f },
            Calves = new BodyRestrictions.ValueRestrictions<float> { Min = 0.3f, Max = 4.5f },
            Dick = new BodyRestrictions.ValueRestrictions<float> { Min = 0.7f, Max = 1.5f },
            Hips = new BodyRestrictions.ValueRestrictions<float> { Min = 0.3f, Max = 3.5f },
            Muscle = new BodyRestrictions.ValueRestrictions<float> { Min = 0.0f, Max = 2.5f },
            Thighs = new BodyRestrictions.ValueRestrictions<float> { Min = 0.3f, Max = 4.5f },
            Torso = new BodyRestrictions.ValueRestrictions<float> { Min = 0.3f, Max = 2.5f },
        };

        return restrictions;
    }

    private static List<BodyProfile> GetBodyProfiles()
    {
        var list = new List<BodyProfile> {
            new() {
                Id      = 1,
                Name    = "Slim",
                Areola  = new()  { Base = 2, Variation = 1},
                Arms    = new() { Base = 0.8f, Variation = 0.2f},
                Belly   = new() { Base = 0.6f, Variation = 0.2f},
                Biceps  = new() { Base = 0.7f, Variation = 0.2f},
                Boobs   = new() { Base = 0.9f, Variation = 0.2f},
                Booty   = new() { Base = 0.8f, Variation = 0.2f},
                Calves  = new() { Base = 0.9f, Variation = 0.2f},
                Dick    = new() { Base = 1.0f, Variation = 0.2f},
                Hips    = new() { Base = 0.8f, Variation = 0.2f},
                Muscle  = new() { Base = 0.3f, Variation = 0.2f},
                Torso   = new() { Base = 0.9f, Variation = 0.2f},
                Thighs  = new() { Base = 0.9f, Variation = 0.2f}
            },
            new() {
                Id      = 2,
                Name    = "Athletic",
                Areola  = new()  { Base = 3, Variation = 1},
                Arms    = new() { Base = 1.5f, Variation = 0.4f},
                Belly   = new() { Base = 0.7f, Variation = 0.2f},
                Biceps  = new() { Base = 2.0f, Variation = 0.7f},
                Boobs   = new() { Base = 1.0f, Variation = 0.2f},
                Booty   = new() { Base = 1.1f, Variation = 0.3f},
                Calves  = new() { Base = 1.5f, Variation = 0.4f},
                Dick    = new() { Base = 1.1f, Variation = 0.2f},
                Hips    = new() { Base = 1.0f, Variation = 0.3f},
                Muscle  = new() { Base = 1.8f, Variation = 0.5f},
                Torso   = new() { Base = 1.4f, Variation = 0.4f},
                Thighs  = new() { Base = 1.6f, Variation = 0.5f}
            },
            new() {
                Id      = 3,
                Name    = "Average",
                Areola  = new()  { Base = 3, Variation = 2},
                Arms    = new() { Base = 1.2f, Variation = 0.3f},
                Belly   = new() { Base = 1.2f, Variation = 0.4f},
                Biceps  = new() { Base = 1.3f, Variation = 0.5f},
                Boobs   = new() { Base = 1.1f, Variation = 0.2f},
                Booty   = new() { Base = 1.1f, Variation = 0.3f},
                Calves  = new() { Base = 1.2f, Variation = 0.3f},
                Dick    = new() { Base = 1.1f, Variation = 0.3f},
                Hips    = new() { Base = 1.2f, Variation = 0.3f},
                Muscle  = new() { Base = 0.8f, Variation = 0.4f},
                Torso   = new() { Base = 1.2f, Variation = 0.3f},
                Thighs  = new() { Base = 1.3f, Variation = 0.4f}
            },
            new() {
                Id      = 4,
                Name    = "Chubby",
                Areola  = new()  { Base = 4, Variation = 2},
                Arms    = new() { Base = 1.8f, Variation = 0.5f},
                Belly   = new() { Base = 2.2f, Variation = 0.7f},
                Biceps  = new() { Base = 1.5f, Variation = 0.5f},
                Boobs   = new() { Base = 1.3f, Variation = 0.3f},
                Booty   = new() { Base = 1.4f, Variation = 0.3f},
                Calves  = new() { Base = 1.7f, Variation = 0.5f},
                Dick    = new() { Base = 1.1f, Variation = 0.3f},
                Hips    = new() { Base = 1.6f, Variation = 0.4f},
                Muscle  = new() { Base = 0.6f, Variation = 0.3f},
                Torso   = new() { Base = 1.8f, Variation = 0.5f},
                Thighs  = new() { Base = 2.0f, Variation = 0.6f}
            },
            new() {
                Id      = 5,
                Name    = "Fat",
                Areola  = new()  { Base = 5, Variation = 2},
                Arms    = new() { Base = 2.5f, Variation = 0.8f},
                Belly   = new() { Base = 3.2f, Variation = 0.8f},
                Biceps  = new() { Base = 1.8f, Variation = 0.6f},
                Boobs   = new() { Base = 1.5f, Variation = 0.3f},
                Booty   = new() { Base = 1.5f, Variation = 0.3f},
                Calves  = new() { Base = 2.3f, Variation = 0.7f},
                Dick    = new() { Base = 1.0f, Variation = 0.3f},
                Hips    = new() { Base = 2.2f, Variation = 0.6f},
                Muscle  = new() { Base = 0.5f, Variation = 0.3f},
                Torso   = new() { Base = 2.2f, Variation = 0.6f},
                Thighs  = new() { Base = 2.8f, Variation = 0.8f}
            },
        };
        return list;
    }

    private static List<EnemyEthnicity> GetEnemyEthnicities()
    {
        List<EnemyEthnicity> list = [];

        list.Add(new EnemyEthnicity
        {
            Id = 0,
            Name = "Any human",
            RandomEthnicity = [1, 2, 3, 4]
        });
        list.Add(new EnemyEthnicity
        {
            Id = 1,
            Name = "White human",

            BodyWeights = new Dictionary<string, int>() {
                { "Slim", 20 },
                { "Athletic", 25 },
                { "Average", 30 },
                { "Chubby", 15 },
                { "Fat", 10 },
            },

            EyesColors = [],
            HairColors = [],
            SkinTones = [
                new EnemyEthnicity.Color {
                    R = 1.0f,
                    G = 1.0f,
                    B = 1.0f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.98490566f,
                    G = 0.9605153f,
                    B = 0.9198647f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.932075440f,
                    G = 0.89697930f,
                    B = 0.84238520f,
                    A = 0.0f,
                },
                new EnemyEthnicity.Color {
                    R = 0.894339560f,
                    G = 0.83069920f,
                    B = 0.727283660f,
                    A = 0.0f,
                },
                new EnemyEthnicity.Color {
                    R = 0.84905660f,
                    G = 0.78607820f,
                    B = 0.67123530f,
                    A = 0.0f,
                }
            ],
            MakeUpColors = [],

            FaceStyle = [],
            EyesStyle = [],
            NoseStyle = [],
            BrowStyle = [],
            MouthStyle = [],
            MouthLength = [],
            LipsForward = [],
            LipsSize = [],
            EarsStyle = [0],
        });
        list.Add(new EnemyEthnicity
        {
            Id = 2,
            Name = "Latin human",

            BodyWeights = new Dictionary<string, int>() {
                { "Slim", 18 },
                { "Athletic", 27 },
                { "Average", 28 },
                { "Chubby", 17 },
                { "Fat", 10 },
            },

            EyesColors = [],
            HairColors = [],
            SkinTones = [
                new EnemyEthnicity.Color{
                    R = 0.89433956f,
                    G = 0.8306992f,
                    B = 0.72728366f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color{
                    R = 0.8490566f,
                    G = 0.7860782f,
                    B = 0.6712353f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color{
                    R = 0.7735849f,
                    G = 0.7222288f,
                    B = 0.63492346f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color{
                    R = 0.735849f,
                    G = 0.64512044f,
                    B = 0.50398713f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color{
                    R = 0.6981132f,
                    G = 0.5730826f,
                    B = 0.3780349f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.5396226f,
                    G = 0.4466827f,
                    B = 0.2881381f,
                    A = 0.0f
                }
            ],
            MakeUpColors = [],

            FaceStyle = [],
            EyesStyle = [],
            NoseStyle = [],
            BrowStyle = [],
            MouthStyle = [],
            MouthLength = [],
            LipsForward = [],
            LipsSize = [],
            EarsStyle = [0],
        });
        list.Add(new EnemyEthnicity
        {
            Id = 3,
            Name = "Black human",

            BodyWeights = new Dictionary<string, int>() {
                { "Slim", 18 },
                { "Athletic", 32 },
                { "Average", 25 },
                { "Chubby", 15 },
                { "Fat", 10 },
            },

            EyesColors = [],
            HairColors = [],
            SkinTones = [
                new EnemyEthnicity.Color {
                    R = 0.4716981f,
                    G = 0.37851143f,
                    B = 0.23050907f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.27450982f,
                    G = 0.2f,
                    B = 0.078431375f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.20784314f,
                    G = 0.13725491f,
                    B = 0.023529412f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.554717f,
                    G = 0.21552648f,
                    B = 0.15385546f,
                    A = 0.0f
                }
            ],
            MakeUpColors = [],

            FaceStyle = [],
            EyesStyle = [],
            NoseStyle = [],
            BrowStyle = [],
            MouthStyle = [],
            MouthLength = [],
            LipsForward = [],
            LipsSize = [],
            EarsStyle = [0],
        });
        list.Add(new EnemyEthnicity
        {
            Id = 4,
            Name = "Asian human",

            BodyWeights = new Dictionary<string, int>() {
                { "Slim", 25 },
                { "Athletic", 25 },
                { "Average", 28 },
                { "Chubby", 12 },
                { "Fat", 10 },
            },

            EyesColors = [],
            HairColors = [],
            SkinTones = [],
            MakeUpColors = [],

            FaceStyle = [],
            EyesStyle = [],
            NoseStyle = [],
            BrowStyle = [],
            MouthStyle = [],
            MouthLength = [],
            LipsForward = [],
            LipsSize = [],
            EarsStyle = [0],
        });

        list.Add(new EnemyEthnicity
        {
            Id = 5,
            Name = "Any elf",
            RandomEthnicity = [6, 7, 8, 9]
        });
        list.Add(new EnemyEthnicity
        {
            Id = 6,
            Name = "High elf",

            BodyWeights = new Dictionary<string, int>() {
                { "Slim", 55 },
                { "Athletic", 35 },
                { "Average", 8 },
                { "Chubby", 2 },
                { "Fat", 1 },
            },

            EyesColors = [],
            HairColors = [],
            SkinTones = [
                new EnemyEthnicity.Color {
                    R = 1.0f,
                    G = 1.0f,
                    B = 1.0f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.98490566f,
                    G = 0.9605153f,
                    B = 0.9198647f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.932075440f,
                    G = 0.89697930f,
                    B = 0.84238520f,
                    A = 0.0f,
                },
                new EnemyEthnicity.Color {
                    R = 0.894339560f,
                    G = 0.83069920f,
                    B = 0.727283660f,
                    A = 0.0f,
                },
                new EnemyEthnicity.Color {
                    R = 0.84905660f,
                    G = 0.78607820f,
                    B = 0.67123530f,
                    A = 0.0f,
                }
            ],
            MakeUpColors = [],

            FaceStyle = [],
            EyesStyle = [],
            NoseStyle = [],
            BrowStyle = [],
            MouthStyle = [],
            MouthLength = [],
            LipsForward = [],
            LipsSize = [],
            EarsStyle = [2, 3, 4, 5],
        });
        list.Add(new EnemyEthnicity
        {
            Id = 7,
            Name = "Latin elf",

            BodyWeights = new Dictionary<string, int>() {
                { "Slim", 45 },
                { "Athletic", 35 },
                { "Average", 12 },
                { "Chubby", 6 },
                { "Fat", 2 },
            },

            EyesColors = [],
            HairColors = [],
            SkinTones = [
                new EnemyEthnicity.Color {
                    R = 0.89433956f,
                    G = 0.8306992f,
                    B = 0.72728366f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.8490566f,
                    G = 0.7860782f,
                    B = 0.6712353f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.7735849f,
                    G = 0.7222288f,
                    B = 0.63492346f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.735849f,
                    G = 0.64512044f,
                    B = 0.50398713f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.6981132f,
                    G = 0.5730826f,
                    B = 0.3780349f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.5396226f,
                    G = 0.4466827f,
                    B = 0.2881381f,
                    A = 0.0f
                }
            ],
            MakeUpColors = [],

            FaceStyle = [],
            EyesStyle = [],
            NoseStyle = [],
            BrowStyle = [],
            MouthStyle = [],
            MouthLength = [],
            LipsForward = [],
            LipsSize = [],
            EarsStyle = [2, 3, 4, 5],
        });
        list.Add(new EnemyEthnicity
        {
            Id = 8,
            Name = "Drow elf",

            BodyWeights = new Dictionary<string, int>() {
                { "Slim", 40 },
                { "Athletic", 50 },
                { "Average", 8 },
                { "Chubby", 2 },
                { "Fat", 1 },
            },

            EyesColors = [],
            HairColors = [],
            SkinTones = [
                new EnemyEthnicity.Color {
                    R = 0.5137255f,
                    G = 0.32156864f,
                    B = 0.60784316f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.48235294f,
                    G = 0.4117647f,
                    B = 0.654902f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.36078432f,
                    G = 0.2901961f,
                    B = 0.5411765f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.30588236f,
                    G = 0.5294118f,
                    B = 0.28627452f,
                    A = 0.0f
                }
            ],
            MakeUpColors = [],

            FaceStyle = [],
            EyesStyle = [],
            NoseStyle = [],
            BrowStyle = [],
            MouthStyle = [],
            MouthLength = [],
            LipsForward = [],
            LipsSize = [],
            EarsStyle = [2, 3, 4, 5],
        });
        list.Add(new EnemyEthnicity
        {
            Id = 9,
            Name = "Red elf",

            BodyWeights = new Dictionary<string, int>() {
                { "Slim", 35 },
                { "Athletic", 35 },
                { "Average", 20 },
                { "Chubby", 8 },
                { "Fat", 2 },
            },

            EyesColors = [],
            HairColors = [],
            SkinTones = [
                new EnemyEthnicity.Color {
                    R = 0.5245282f,
                    G = 0.12973839f,
                    B = 0.058390833f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.45490196f,
                    G = 0.07450981f,
                    B = 0.0f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.4056604f,
                    G = 0.024536664f,
                    B = 0.0f,
                    A = 0.0f
                },
                new EnemyEthnicity.Color {
                    R = 0.61960787f,
                    G = 0.42745098f,
                    B = 0.70980394f,
                    A = 0.0f
                }
            ],
            MakeUpColors = [],

            FaceStyle = [],
            EyesStyle = [],
            NoseStyle = [],
            BrowStyle = [],
            MouthStyle = [],
            MouthLength = [],
            LipsForward = [],
            LipsSize = [],
            EarsStyle = [2, 3, 4, 5],
        });

        list.Add(new EnemyEthnicity
        {
            Id = 10,
            Name = "Random",
            RandomEthnicity = [1, 2, 3, 4, 6, 7, 8, 9]
        });

        return list;
    }

    private static EnemyEthnicity GetEnemyEthnicity(int id)
    {
        EnemyEthnicity enemyEthnicity = EnemyEthnicities.Find(x => x.Id == id);
        enemyEthnicity ??= RandomUtils.Item(EnemyEthnicities.Where(x => !x.IsRandomEthnicity));

        if (enemyEthnicity.IsRandomEthnicity)
        {
            enemyEthnicity = RandomUtils.Item(EnemyEthnicities.Where(x => enemyEthnicity.RandomEthnicity.Contains(x.Id)));
        }

        return enemyEthnicity;
    }

    public static BodyProfile GetBodyProfile(Dictionary<BodyProfile, int> weights)
    {
        int total = 0;

        foreach (int w in weights.Values)
        {
            total += w;
        }

        int roll = RandomUtils.Int32(total);

        int current = 0;

        foreach (KeyValuePair<BodyProfile, int> pair in weights)
        {
            current += pair.Value;

            if (roll <= current)
            {
                return pair.Key;
            }
        }

        // fallback (на всякий случай)
        return BodyProfiles.RandomItem();
    }
}
