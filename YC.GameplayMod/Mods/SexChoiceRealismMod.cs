using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using Il2Cpp;

using Il2CppInterop.Runtime;

using UnityEngine;
using UnityEngine.SceneManagement;

using YC.GameplayMod.Components;
using YC.GameplayMod.Configs;
using YC.GameplayMod.Models;

namespace YC.GameplayMod.Mods;
internal class SexChoiceRealismMod {
    #region Configuration
    internal static bool Enabled;
    internal static bool UpdateMoves;
    internal static bool UsePlayerPreferredPositions;
    #endregion

    #region States
    internal static bool IsModActive => Enabled;
    internal static List<SexMoveExtended> SexMoves { get; set; } = [];
    internal static List<PersonalitySexTags> PersonalitySexTypes { get; set; } = [];
    #endregion

    #region Storage
    internal static CharacterDataa Character => CharacterDataa.Instance;
    internal static SexSystem SexSystem;
    internal static int LastSexType = 0;
    internal static SexMoveExtended LastMove;
    #endregion

    internal static void Load(ModConfig config) {
        try {
            Enabled = config.SexChoiceRealism.Enabled;
            UpdateMoves = config.SexChoiceRealism.UpdateMoves;
            UsePlayerPreferredPositions = config.SexChoiceRealism.UsePlayerPreferredPositions;

        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void Prepare() {
        try {
            SexSystem = Zessentials.Instance.gameObject.GetComponentWithCast<SexSystem>();

            bool fromFile = false;
            if (JsonUtils.TryDeserialize(Plugin.PluginResources, "SexMoves.json", out List<SexMoveExtended> extendedSexMoves))                 
                fromFile = true;

            if (!fromFile) {
                List<SexMoveExtended> poses = GetSexMoves();
                poses.Sort();
                if (JsonUtils.TrySerialize(Plugin.PluginResources, "SexMoves.json", poses)) {
                    extendedSexMoves = poses;
                    Plugin.Log.Info($"SexMoves.json was created in {Plugin.PluginResources}");
                    File.WriteAllText($"{Plugin.PluginResources}/KnownIds.txt", string.Join(", ", extendedSexMoves.Select(m => m.ID)));
                } else {
                    Plugin.Log.Info($"SexMoves.json was not created");
                }
            } else if (UpdateMoves) {
                List<SexMoveExtended> poses = GetSexMoves();
                foreach (var item in poses) {
                    var move = extendedSexMoves.FirstOrDefault(p => p.ID == item.ID);
                    if (move is null)
                    if (!extendedSexMoves.Contains(item))
                        extendedSexMoves.Add(item);
                    else 
                        move.Update(item);
                }

                extendedSexMoves.Sort();

                if (JsonUtils.TrySerialize(Plugin.PluginResources, "SexMoves.json", extendedSexMoves)) {
                    Plugin.Log.Info($"SexMoves.json was updated in {Plugin.PluginResources}");
                    File.WriteAllText($"{Plugin.PluginResources}/KnownIds.txt", string.Join(", ", extendedSexMoves.Select(m => m.ID)));
                } else {
                    Plugin.Log.Info($"SexMoves.json was not updated");
                }
            }

            if (extendedSexMoves.Count > 0)
                SexMoves.Clear();

            foreach (var sexMove in extendedSexMoves) {
                if (!Character.statusDATA.DislikedSex.Contains(sexMove.ID) && !Character.statusDATA.DislikedThreesomeSex.Contains(sexMove.ID))
                    SexMoves.Add(sexMove);
            }

            if (!JsonUtils.TryDeserialize(Plugin.PluginResources, "PersonalitySexTags.json", out List<PersonalitySexTags> personalitySexTypes)) {
                personalitySexTypes = GetPersonalitySexTypes();
                JsonUtils.TrySerialize(Plugin.PluginResources, "PersonalitySexTags.json", personalitySexTypes);
            }
            PersonalitySexTypes = personalitySexTypes;
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void SetSexID(SexEncounter sexEncounter) {
        try {
            if (!Enabled || SceneManager.GetActiveScene().buildIndex < 2) {
                Plugin.Log.Info("Exit due execute condition");
                return;
            }

            if (sexEncounter is null) {
                Plugin.Log.Info("Exit due SexEncounter is NULL");
                return;
            }

            if (SexMoves.Count == 0) {
                Plugin.Log.Info("Exit due EMPTY SexPositions");
                return;
            }

            if (!sexEncounter.CasterSex.IsMale && !sexEncounter.CasterSex.IsFuta) {
                Plugin.Log.Info("Reset Caster Active/Futa for strapon");
                sexEncounter.CasterActive = false;
                sexEncounter.CasterFuta = false;
            }

            if (!sexEncounter.TargetSex.IsMale && !sexEncounter.TargetSex.IsFuta) {
                Plugin.Log.Info("Reset Target Active/Futa for strapon");
                sexEncounter.TargetActive = false;
                sexEncounter.TargetFuta = false;
            }

            var move = GetSexMove(sexEncounter);
            if (move is null) {
                Plugin.Log.Info("Exit due SexMove is null");
                return;
            }

            Plugin.Log.Info($"Set SexMove: ID: {move.ID}({move.Type}) Name: '{move.Name}'");
            sexEncounter.SexType = move.Type;
            sexEncounter.SexID = move.ID;
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
        }
    }

    internal static void SetThreesomeSexID(SexEncounter sexEncounter) {
        try {
            if (!Enabled || SceneManager.GetActiveScene().buildIndex < 2) {
                Plugin.Log.Info("Exit due execute condition");
                return;
            }

            if (sexEncounter is null) {
                Plugin.Log.Info("Exit due SexSystem is NULL");
                return;
            }

            if (SexMoves.Count == 0) {
                Plugin.Log.Info("Exit due EMPTY SexPositions");
                return;
            }

            int cums = Mathf.Min(sexEncounter.CasterSex.characterAttributes.cumsInSuccession, sexEncounter.TargetSex.characterAttributes.cumsInSuccession);
            if (!sexEncounter.CasterSex.IsMale && !sexEncounter.CasterSex.IsFuta) {
                Plugin.Log.Info("Reset Caster Active/Futa for strapon");
                sexEncounter.CasterActive = false;
                sexEncounter.CasterFuta = false;
            }

            if (!sexEncounter.TargetSex.IsMale && !sexEncounter.TargetSex.IsFuta) {
                Plugin.Log.Info("Reset Target Active/Futa for strapon");
                sexEncounter.TargetActive = false;
                sexEncounter.TargetFuta = false;
            }

            if (!sexEncounter.AssistSex.IsMale && !sexEncounter.AssistSex.IsFuta) {
                Plugin.Log.Info("Reset Assist Active/Futa for strapon");
                sexEncounter.AssistActive = sexEncounter.CasterActive;
                sexEncounter.AssistFuta = sexEncounter.CasterFuta;
            }

            var move = GetThreesomeSexMove(sexEncounter);
            if (move is null) {
                Plugin.Log.Info("Exit due SexMove is null");
                return;
            }

            Plugin.Log.Info($"Set SexMove: ID: {move.ID}({move.Type}) Name: '{move.Name}'");
            sexEncounter.SexType = move.Type;
            sexEncounter.SexID = move.ID;
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
        }
    }

    private static List<SexMoveExtended> GetSexMoves() {
        Plugin.Log.Info("Creating SexMoves.json...");
        List<SexMoveExtended> poses = [];

        if (Zessentials.Instance.gameObject.TryGetComponentWithCast( out CombatHolder holder )) {
            Plugin.Log.Info("Get SexMoves from CombatHolder");
            foreach (var sexMove in holder.Sexmoves) {
                if ( holder.AvailableSexMoves.Contains(sexMove.ID) ) {
                    var move = SexMoveExtended.FromSexMove(sexMove);
                    if (move is not null)
                        poses.Add(move);
                }
            }

            foreach (var sexMove in holder.Threesomemoves) {
                var move = SexMoveExtended.FromSexMove(sexMove);
                if (move is not null)
                    poses.Add(move);
            }

            Plugin.Log.Info($"Add {poses.Count} poses");
        } else {
            Plugin.Log.Info("Try find SexMoves in Resources");
            var sexMoveObj = Resources.FindObjectsOfTypeAll( Il2CppType.From( typeof(SexMove) ) );
            foreach (var moveObj in sexMoveObj) {
                var sexMove = moveObj.TryCast<SexMove>();
                if (sexMove is not null && sexMove.ID > 0 && sexMove.Type > 0) {
                    var move = SexMoveExtended.FromSexMove(sexMove);
                    if (move is not null)
                        poses.Add(move);
                }
            }
            Plugin.Log.Info($"Add {poses.Count}/{sexMoveObj.Count}");
        }

        poses.Sort();

        return poses;
    }

    private static List<PersonalitySexTags> GetPersonalitySexTypes() {
        List<PersonalitySexTags> personalitySexTypes = [];
        personalitySexTypes.Add(new PersonalitySexTags() {
            Id = 1,
            Name = "Balanced",
            PreferredTags = SexTag.Universal | SexTag.Sensual | SexTag.Service,
            NeutralTags = SexTag.Spanking | SexTag.Dominant,
            AvoidTags = SexTag.Smothering | SexTag.Wresting
        });
        personalitySexTypes.Add(new PersonalitySexTags() {
            Id = 2,
            Name = "Dominant",
            PreferredTags = SexTag.Dominant | SexTag.Spanking | SexTag.Smothering | SexTag.Wresting,
            NeutralTags = SexTag.Sensual | SexTag.Universal,
            AvoidTags = SexTag.Service
        });
        personalitySexTypes.Add(new PersonalitySexTags() {
            Id = 3,
            Name = "Defensive",
            PreferredTags = SexTag.Service | SexTag.Sensual | SexTag.Universal,
            NeutralTags = SexTag.Smothering,
            AvoidTags = SexTag.Dominant | SexTag.Wresting | SexTag.Spanking
        });
        personalitySexTypes.Add(new PersonalitySexTags() {
            Id = 4,
            Name = "Passionate",
            PreferredTags = SexTag.Sensual | SexTag.Dominant | SexTag.Spanking | SexTag.Smothering,
            NeutralTags = SexTag.Universal,
            AvoidTags = SexTag.Service | SexTag.Wresting
        });
        personalitySexTypes.Add(new PersonalitySexTags() {
            Id = 5,
            Name = "Submissive",
            PreferredTags = SexTag.Service | SexTag.Sensual | SexTag.Universal,
            NeutralTags = SexTag.Smothering,
            AvoidTags = SexTag.Dominant | SexTag.Wresting | SexTag.Spanking
        });
        personalitySexTypes.Add(new PersonalitySexTags() {
            Id = 6,
            Name = "Trickster",
            PreferredTags = SexTag.Wresting | SexTag.Smothering | SexTag.Spanking,
            NeutralTags = SexTag.Dominant | SexTag.Service | SexTag.Sensual,
            AvoidTags = SexTag.Universal
        });

        return personalitySexTypes;
    }

    private static SexMoveExtended GetSexMove(SexEncounter sexEncounter) {
        List<(SexMoveExtended move, int score)> sexMoves = GetCharacterSexMoves( sexEncounter );

        if (sexMoves.Count <= 0)
            return null;

        var move = ChooseWeightedRandom(sexMoves);
        LastSexType = move.Type;
        LastMove = move;
        return move;
    }

    private static SexMoveExtended GetThreesomeSexMove(SexEncounter sexEncounter ) {
        List<(SexMoveExtended move, int score)> sexMoves = GetCharacterSexMoves( sexEncounter );
        
        if (sexMoves.Count <= 0)
            return null;

        var move = ChooseWeightedRandom(sexMoves);
        LastSexType = move.Type;
        LastMove = move;
        return move;
    }

    private static List<(SexMoveExtended move, int score)> GetCharacterSexMoves(SexEncounter sexEncounter ) {
        List<(SexMoveExtended move, int score)> sexMoves = [];
        CharacterGender casterGender = GetCharacterGender( sexEncounter.CasterSex );
        CharacterRole casterRole = sexEncounter.CasterActive ? CharacterRole.Active : CharacterRole.Passive;
        CharacterStatus casterStatus = GetCharacterStatus( sexEncounter.CasterAttributes );
        CharacterGender targetGender = GetCharacterGender( sexEncounter.TargetSex );
        CharacterRole targetRole = sexEncounter.TargetActive ? CharacterRole.Active : CharacterRole.Passive;
        CharacterStatus targetStatus = GetCharacterStatus( sexEncounter.TargetAttributes );
        CharacterGender assistGender = sexEncounter.IsThreesome ? GetCharacterGender( sexEncounter.AssistSex ) : CharacterGender.Any;
        CharacterRole assistRole = sexEncounter.IsThreesome 
            ? sexEncounter.AssistActive ? CharacterRole.Active : CharacterRole.Passive
            : CharacterRole.Any;

        bool allowCommand = targetStatus.HasFlag(CharacterStatus.Collared) || targetStatus.HasFlag(CharacterStatus.Aroused);
        PositionGroup positionGroup = ChooseChanceMixed(sexEncounter, casterStatus, targetStatus);
        bool isThreesome = sexEncounter.IsThreesome;

        int personalityId = sexEncounter.CasterSex.IsPlayer
            ? CharacterDataa.Instance.adultSettingsDATA.SexGameplayAI
            : sexEncounter.CasterAttributes.enemyData?.statsDATA.EnemyPersonality ?? 0;
        PersonalitySexTags sexType = PersonalitySexTypes.FirstOrDefault( t => t.Id == personalityId )?.UpdateByStatus(casterStatus);
                
        Plugin.Log.Info($"- Caster Gender: {casterGender}; Caster Role: {casterRole}; Caster Status: {casterStatus}");
        Plugin.Log.Info($"- Target: Gender: {targetGender}; Caster Role: {targetRole}; Caster Status: {targetStatus}; Target charmed: {(allowCommand ? "Yes" : "No")}");
        if (sexEncounter.IsThreesome)
            Plugin.Log.Info($"- Assist: Gender: {assistGender}; Caster Role: {assistRole}");

        foreach (var sexMove in SexMoves) {
            if (sexMove.IsDisabled)
                continue;

            if (!positionGroup.HasFlag( sexMove.PositionGroup ))
                continue;

            if (LastMove is not null && sexMove.ID == LastMove.ID)
                continue;

            if (sexMove.IsThreesome != isThreesome)
                continue;

            if (sexMove.IsCommand && !sexMove.IsPerform){
                if (!allowCommand)
                    continue;
            }

            if (!CheckMainRolesAndGenders(sexMove, casterGender, casterRole, targetGender, targetRole))
                continue;

            if (sexMove.IsThreesome && (!sexMove.AssistGender.HasFlag(assistGender) || !sexMove.AssistRole.HasFlag(assistRole)))
                continue;

            int score = 0;
            if (sexEncounter.CasterSex.IsPlayer && UsePlayerPreferredPositions) {
                if (!Character.statusDATA.PreferredSex.Contains(sexMove.ID))
                    continue;

                score = 1;
            } else {
                score = GetPositionScore(sexMove.SexTags, sexType);
            }

            sexMoves.Add((sexMove, score));
        }
        Plugin.Log.Info($"Selected Sex moves: {sexMoves.Count} / {SexMoves.Count}");

        return sexMoves;
    }

    static PositionGroup ChooseChanceMixed(SexEncounter sexEncounter, CharacterStatus casterStatus, CharacterStatus targetStatus) {
        try {
            int sexChance = 40;
            if ( sexEncounter.CasterSex.gameObject.TryGetComponentWithCast(out GameplayModComponent casterComponent) 
                && sexEncounter.TargetSex.gameObject.TryGetComponentWithCast(out GameplayModComponent targetComponent)) {
                int casterCumsInSuccession  = casterComponent.SexInteractions;
                int casterCurrentPleasure   = casterComponent.Attributes.currentPleasure;
                int targetCumsInSuccession  = targetComponent.SexInteractions;
                int targetCurrentPleasure   = targetComponent.Attributes.currentPleasure;

                int delta                   = casterCumsInSuccession - targetCumsInSuccession;
                int baseChance              = Math.Abs(delta) * 10;
                int casterBonus             = casterCumsInSuccession * 3;
                int casterStatusBonus       = casterStatus.HasFlag(CharacterStatus.Collared) || casterStatus.HasFlag(CharacterStatus.Aroused) || casterStatus.HasFlag(CharacterStatus.Charmed) ? 5 : 0;
                int targetBonus             = targetCumsInSuccession * 5;
                int targetStatusBonus       = targetStatus.HasFlag(CharacterStatus.Collared) || targetStatus.HasFlag(CharacterStatus.Aroused) ? 5 : 0;
                int plesureState            = Math.Min(casterCurrentPleasure, targetCurrentPleasure) / 2500;
                int plesureBonus            = Convert.ToInt32(Math.Pow(5, plesureState));

                sexChance = Math.Clamp(baseChance + plesureBonus + targetBonus + targetStatusBonus + casterStatusBonus - casterBonus, 5, 95 );
            }

            PositionGroup group = RandomUtils.Chance(sexChance) ? PositionGroup.Sex : PositionGroup.Foreplay;
            Plugin.Log.Info($"Select Sex moves: Chance: {sexChance} => Type: {group}");
            return group;
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
            return PositionGroup.Any;
        }
    }

    static CharacterGender GetCharacterGender( CharacterSex characterSex ) {
        CharacterGender gender;
        if (characterSex.IsMale)
            gender = CharacterGender.Male;
        else if (characterSex.IsFuta)
            gender = CharacterGender.Futa;
        else
            gender = CharacterGender.Female;

        return gender;
    }

    static CharacterStatus GetCharacterStatus(CharacterAttributes characterAttributes) {
        CharacterStatus status = CharacterStatus.None;

        if (characterAttributes.activeBuffs.Count > 0) {
            foreach (var buffUI in characterAttributes.activeBuffs) {
                if (buffUI.buff.buffName.Contains("Aroused"))
                    status |= CharacterStatus.Aroused;

                if (buffUI.buff.buffName.Contains("Collar"))
                    status |= CharacterStatus.Collared;

                if (buffUI.buff.buffName.Contains("Charmed"))
                    status |= CharacterStatus.Charmed;
            }
        }

        return status;
    }

    static bool CheckMainRolesAndGenders(SexMoveExtended sexMove, CharacterGender casterGender, CharacterRole casterRole, CharacterGender targetGender, CharacterRole targetRole) {
        // Обычная проверка
        if (sexMove.CasterGender.HasFlag(casterGender) && sexMove.CasterRole.HasFlag(casterRole)
            && sexMove.TargetGender.HasFlag(targetGender) && sexMove.TargetRole.HasFlag(targetRole)) {
            return true;
        }

        // Реверс только если поза универсальная
        if (sexMove.IsCommand && sexMove.IsPerform) {
            if (sexMove.CasterGender.HasFlag(targetGender) && sexMove.CasterRole.HasFlag(targetRole)
                && sexMove.TargetGender.HasFlag(casterGender) && sexMove.TargetRole.HasFlag(casterRole)) {
                return true;
            }
        }

        return false;
    }

    static int GetPositionScore(SexTag positionTags, PersonalitySexTags personality) {
        if (personality == null)
            return 1;

        int preferredScore = 5;
        int neutralScore = 2;
        int avoidScore = 3;
        bool hasFlags = false;
        int score = 0;

        foreach (SexTag tag in Enum.GetValues(typeof(SexTag))) {
            if (tag == SexTag.None)
                continue;

            if (!positionTags.HasFlag(tag))
                continue;

            if (positionTags.HasFlag(tag)) {
                if (personality.PreferredTags.HasFlag(tag)) {
                    hasFlags = true;
                    score += preferredScore;
                } else if (personality.NeutralTags.HasFlag(tag)) {
                    hasFlags = true;
                    score += neutralScore;
                } else if (personality.AvoidTags.HasFlag(tag)) {
                    hasFlags = true;
                    score -= avoidScore;
                }
            }
        }

        if (!hasFlags) 
            score = neutralScore;

        return score;
    }

    static SexMoveExtended ChooseWeightedRandom(List<(SexMoveExtended move, int score)> moves) {
        int totalWeight = moves.Sum(m => m.score);
        if (totalWeight == 0)
            return null;

        int roll = UnityEngine.Random.Range(0, totalWeight);
        int current = 0;

        foreach (var (move, score) in moves) {
            current += score;
            if (roll < current)
                return move;
        }

        return null;
    }
}