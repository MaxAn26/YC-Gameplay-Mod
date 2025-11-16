using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using Il2Cpp;

using Il2CppInterop.Runtime;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using YC.GameplayMod.Components;
using YC.GameplayMod.Configs;
using YC.GameplayMod.Models;

using static MelonLoader.MelonLogger;

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
                UpdateMoves = false;
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

    internal static int GetSexId(CharacterSex casterSex, CharacterSex targetSex, CharacterSex assistSex = null) {
        if (!Enabled || SceneManager.GetActiveScene().buildIndex < 2) {
            Plugin.Log.Info("Exit due execute condition");
            return -1; //-1
        }

        if (casterSex is null || targetSex is null) {
            Plugin.Log.Info("Exit due casterSex OR targetSex is NULL");
            return -1;
        }

        if (SexMoves.Count == 0) {
            Plugin.Log.Info("Exit due EMPTY SexPositions");
            return -1;
        }

        if (!casterSex.gameObject.TryGetComponentWithCast(out GameplayModComponent casterComponent)
            || !targetSex.gameObject.TryGetComponentWithCast(out GameplayModComponent targetComponent)) {
            return -1;
        }

        // Reset Active state
        casterSex.IsActive = casterComponent.IsActiveRole;
        targetSex.IsActive = targetComponent.IsActiveRole;

        if (assistSex is not null && assistSex.gameObject.TryGetComponentWithCast(out GameplayModComponent _)) {
            assistSex = casterSex.currentSexEncounter.AssistSex;
        }

        if (assistSex is not null) {
            assistSex.IsActive = casterSex.IsActive;
        }

        var (positionGroup, positionAction) = ChooseChanceMixed(casterSex, targetSex);
        if (positionGroup is PositionGroup.Foreplay) {
            if (casterSex.IsActive && !casterSex.IsFuta && !casterSex.IsMale) {
                Plugin.Log.Debug($"Reset Caster role for Foreplay");
                casterSex.IsActive = false;
            }

            if (targetSex.IsActive && !targetSex.IsFuta && !targetSex.IsMale) {
                Plugin.Log.Debug($"Reset Target role for Foreplay");
                targetSex.IsActive = false;
            }

            if (assistSex is not null) {
                assistSex.IsActive = casterSex.IsActive;
            }
        }

        List<(SexMoveExtended move, int score)> sexMoves = GetCharacterSexMoves( positionGroup, positionAction, casterSex, targetSex, assistSex );

        if (sexMoves.Count <= 0) {
            Plugin.Log.Info("Exit due EMPTY character sexMoves");
            return -1;
        }

        var move = ChooseWeightedRandom(sexMoves);
        LastMove = move;

        return move is not null ? move.ID : -1;
    }

    internal static bool JoinThreesomeFix(SexEncounter sexEncounter, CharacterAttributes joinedCharacter, int oldSexID) {
        try {
            if (joinedCharacter == null) { return false; }

            CharacterSex casterSex          = sexEncounter.CasterSex;
            CharacterSex currentCasterSex   = sexEncounter.CasterSex;
            CharacterSex targetSex          = sexEncounter.TargetSex;
            CharacterSex currentTargetSex   = sexEncounter.TargetSex;
            CharacterSex assistSex          = joinedCharacter.characterSex;
            CharacterSex currentAssistSex   = joinedCharacter.characterSex;

            if (assistSex.IsPlayer) {
                Plugin.Log.Debug($"Player join to Threesome");
                SexEncounter.print($"Player join to Threesome");
                if (casterSex.characterAttributes.combatAI.isAlly && !casterSex.characterAttributes.CheckForStatus("Charmed")) {
                    casterSex = currentAssistSex;
                    targetSex = currentTargetSex;
                    assistSex = currentCasterSex;
                } else {
                    casterSex = currentAssistSex;
                    targetSex = currentCasterSex;
                    assistSex = currentTargetSex;
                }

                if (CharacterDataa.Instance.currentState == CharacterDataa.BattleState.Fucking) {
                    sexEncounter.battleManager.combatUIManager.EnableSexfightUI();
                } else {
                    sexEncounter.battleManager.combatUIManager.DisableSexfightUI();
                }
            } else if (assistSex.characterAttributes.combatAI.isAlly) {
                Plugin.Log.Debug($"Ally join to Threesome");
                SexEncounter.print($"Ally join to Threesome");
                if (targetSex.IsPlayer) {
                    casterSex = currentTargetSex;
                    targetSex = currentCasterSex;
                    assistSex = currentAssistSex;
                } else if (targetSex.characterAttributes.combatAI.isAlly) {
                    casterSex = currentAssistSex;
                    targetSex = currentCasterSex;
                    assistSex = currentTargetSex;
                } else {
                    casterSex = currentCasterSex;
                    targetSex = currentTargetSex;
                    assistSex = currentAssistSex;
                }
            } else if (assistSex.characterAttributes.combatAI.isElite) {
                Plugin.Log.Debug($"Elite enemy join to Threesome");
                SexEncounter.print($"Elite enemy join to Threesome");
                if (casterSex.IsPlayer || casterSex.characterAttributes.combatAI.isAlly) {
                    casterSex = currentAssistSex;
                    targetSex = currentCasterSex;
                    assistSex = currentTargetSex;
                } else {
                    casterSex = currentAssistSex;
                    targetSex = currentTargetSex;
                    assistSex = currentCasterSex;
                }
            } else {
                Plugin.Log.Debug($"Enemy join to Threesome");
                SexEncounter.print($"Enemy join to Threesome");
                if (casterSex.IsPlayer || casterSex.characterAttributes.combatAI.isAlly) {
                    casterSex = currentTargetSex;
                    targetSex = currentCasterSex;
                    assistSex = currentAssistSex;
                } else {
                    casterSex = currentCasterSex;
                    targetSex = currentTargetSex;
                    assistSex = currentAssistSex;
                }
            }

            assistSex.currentSexEncounter = sexEncounter;
            casterSex.currentSexEncounter = sexEncounter;
            targetSex.currentSexEncounter = sexEncounter;

            sexEncounter.Assist = assistSex.gameObject;
            sexEncounter.AssistSex = assistSex;
            sexEncounter.AssistAnim = assistSex.GetComponent<Animator>();
            sexEncounter.AssistAttributes = assistSex.characterAttributes;
            sexEncounter.AssistMale = assistSex.IsMale;
            sexEncounter.AssistActive = assistSex.IsActive;
            sexEncounter.AssistFuta = assistSex.IsFuta;

            sexEncounter.Caster = casterSex.gameObject;
            sexEncounter.CasterSex = casterSex;
            sexEncounter.CasterAnim = casterSex.GetComponent<Animator>();
            sexEncounter.CasterAttributes = casterSex.characterAttributes;
            sexEncounter.CasterMale = casterSex.IsMale;
            sexEncounter.CasterActive = casterSex.IsActive;
            sexEncounter.CasterFuta = casterSex.IsFuta;

            sexEncounter.Target = targetSex.gameObject;
            sexEncounter.TargetSex = targetSex;
            sexEncounter.TargetAnim = targetSex.GetComponent<Animator>();
            sexEncounter.TargetAttributes = targetSex.characterAttributes;
            sexEncounter.TargetMale = targetSex.IsMale;
            sexEncounter.TargetActive = targetSex.IsActive;
            sexEncounter.TargetFuta = targetSex.IsFuta;

            sexEncounter.IsThreesome = true;
            sexEncounter.ThreesomeIsCasterFriend = true;
            sexEncounter.CasterIsAttacker = true;

            int newSexId = GetSexId(sexEncounter.CasterSex, targetSex, assistSex);
            if (newSexId > 0)
                oldSexID = newSexId;

            sexEncounter.SexID = oldSexID;
            sexEncounter.StartThreesome();

            return true;
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
            return false;
        }
    }

    private static List<SexMoveExtended> GetSexMoves() {
        Plugin.Log.Info("Creating SexMoves.json...");
        List<SexMoveExtended> poses = [];

        if (Zessentials.Instance.gameObject.TryGetComponentWithCast(out CombatHolder holder)) {
            Plugin.Log.Info("Get SexMoves from CombatHolder");
            foreach (var sexMove in holder.Sexmoves) {
                if (holder.AvailableSexMoves.Contains(sexMove.ID)) {
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


    private static CharacterStatus GetCharacterStatus(CharacterAttributes characterAttributes) {
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

    private static (PositionGroup group, PositionActionMode actionMode ) ChooseChanceMixed(CharacterSex casterSex, CharacterSex targetSex) {
        CharacterStatus casterStatus = GetCharacterStatus( casterSex.characterAttributes);
        CharacterStatus targetStatus = GetCharacterStatus( targetSex.characterAttributes );

        try {
            int sexChance = 40;
            if (casterSex.gameObject.TryGetComponentWithCast(out GameplayModComponent casterComponent)
                && targetSex.gameObject.TryGetComponentWithCast(out GameplayModComponent targetComponent)) {
                int casterCumsInSuccession  = casterComponent.SexInteractions;
                int casterCurrentPleasure   = casterComponent.Attributes.currentPleasure;
                int targetCumsInSuccession  = targetComponent.SexInteractions;
                int targetCurrentPleasure   = targetComponent.Attributes.currentPleasure;

                int delta                   = casterCumsInSuccession - targetCumsInSuccession;
                if (delta == 0)
                    delta = casterCumsInSuccession;

                int baseChance              = Math.Abs(delta) * 10;
                int casterBonus             = casterCumsInSuccession * 3;
                int casterStatusBonus       = casterStatus.HasFlag(CharacterStatus.Collared) || casterStatus.HasFlag(CharacterStatus.Aroused) || casterStatus.HasFlag(CharacterStatus.Charmed) ? 5 : 0;
                int targetBonus             = targetCumsInSuccession * 5;
                int targetStatusBonus       = targetStatus.HasFlag(CharacterStatus.Collared) || targetStatus.HasFlag(CharacterStatus.Aroused) ? 5 : 0;
                int plesureState            = Math.Min(casterCurrentPleasure, targetCurrentPleasure) / 2500;
                int plesureBonus            = Convert.ToInt32(Math.Pow(5, plesureState));

                sexChance = Math.Clamp(baseChance + plesureBonus + targetBonus + targetStatusBonus + casterStatusBonus - casterBonus, 5, 95);
            }

            PositionGroup group = RandomUtils.Chance(sexChance) ? PositionGroup.Sex : PositionGroup.Foreplay;
            PositionActionMode positionAction = PositionActionMode.Perform;
            if (targetStatus.HasFlag(CharacterStatus.Collared) || targetStatus.HasFlag(CharacterStatus.Aroused) || targetStatus.HasFlag(CharacterStatus.Charmed))
                positionAction |= PositionActionMode.Command;
            
            Plugin.Log.Info($"Select Sex moves: Chance: {sexChance} => Type: {group}, Mode: {positionAction}");
            return (group, positionAction);
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
            return (PositionGroup.Any, PositionActionMode.Any);
        }
    }

    private static List<(SexMoveExtended move, int score)> GetCharacterSexMoves(PositionGroup positionGroup, PositionActionMode positionAction, CharacterSex casterSex, CharacterSex targetSex, CharacterSex assistSex) {
        List<(SexMoveExtended move, int score)> sexMoves = [];
        CharacterGender casterGender = GetCharacterGender( casterSex );
        CharacterRole casterRole = casterSex.IsActive ? CharacterRole.Active : CharacterRole.Passive;

        CharacterGender targetGender = GetCharacterGender( targetSex );
        CharacterRole targetRole = targetSex.IsActive ? CharacterRole.Active : CharacterRole.Passive;

        bool isThreesome = assistSex is not null && casterSex.currentSexEncounter?.IsThreesome == true;
        CharacterGender assistGender = isThreesome ? GetCharacterGender( assistSex ) : CharacterGender.Any;
        CharacterRole assistRole = isThreesome
            ? assistSex.IsActive ? CharacterRole.Active : CharacterRole.Passive
            : CharacterRole.Any;
        
        int personalityId = casterSex.IsPlayer
            ? CharacterDataa.Instance.adultSettingsDATA.SexGameplayAI
            : casterSex.characterAttributes.enemyData?.statsDATA.EnemyPersonality ?? 0;
        PersonalitySexTags sexType = PersonalitySexTypes.FirstOrDefault( t => t.Id == personalityId )?.UpdateByStatus(GetCharacterStatus(casterSex.characterAttributes));

        foreach (var sexMove in SexMoves) {
            if (sexMove.IsDisabled)
                continue;

            if (!positionGroup.HasFlag(sexMove.PositionGroup))
                continue;

            if ((positionAction & sexMove.PositionAction) == 0)
                continue;

            if (LastMove is not null && sexMove.ID == LastMove.ID)
                continue;

            if (sexMove.IsThreesome != isThreesome)
                continue;

            if (!CheckMainRolesAndGenders(sexMove, casterGender, casterRole, targetGender, targetRole))
                continue;

            if (sexMove.IsThreesome && (!sexMove.AssistGender.HasFlag(assistGender) || !sexMove.AssistRole.HasFlag(assistRole)))
                continue;

            int score = 0;
            if (casterSex.IsPlayer && UsePlayerPreferredPositions) {
                if (!Character.statusDATA.PreferredSex.Contains(sexMove.ID))
                    continue;

                score = 1;
            } else {
                score = GetPositionScore(sexMove.SexTags, sexType);
            }

            sexMoves.Add((sexMove, score));
        }
        Plugin.Log.Debug($"Selected Sex moves: {sexMoves.Count} / {SexMoves.Count}");

        return sexMoves;
    }

    private static CharacterGender GetCharacterGender(CharacterSex characterSex) {
        CharacterGender gender;
        if (characterSex.IsMale)
            gender = CharacterGender.Male;
        else if (characterSex.IsFuta)
            gender = CharacterGender.Futa;
        else
            gender = CharacterGender.Female;

        return gender;
    }

    private static bool CheckMainRolesAndGenders(SexMoveExtended sexMove, CharacterGender casterGender, CharacterRole casterRole, CharacterGender targetGender, CharacterRole targetRole) {
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

    private static int GetPositionScore(SexTag positionTags, PersonalitySexTags personality) {
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

    private static SexMoveExtended ChooseWeightedRandom(List<(SexMoveExtended move, int score)> moves) {
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