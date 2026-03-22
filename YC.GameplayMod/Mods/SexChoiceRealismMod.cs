using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

using BaseMod.Core;
using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using Il2Cpp;

using Il2CppInterop.Runtime;

using MelonLoader;

using UnityEngine;
using UnityEngine.SceneManagement;

using YC.GameplayMod.Components;
using YC.GameplayMod.Models;

namespace YC.GameplayMod.Mods;
internal class SexChoiceRealismMod {
    #region Configuration
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> RandomPersonalityAlly;
    internal static MelonPreferences_Entry<bool> RandomPersonalityElite;
    internal static MelonPreferences_Entry<bool> RandomPersonalityEnemy;
    internal static MelonPreferences_Entry<bool> UpdateMoves;
    internal static MelonPreferences_Entry<bool> UsePlayerPreferredPositions;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static List<SexMoveExtended> SexMoves { get; set; } = [];
    internal static List<PersonalitySexTags> PersonalitySexTypes { get; set; } = [];
    #endregion

    #region Storage
    internal static CharacterDataa Character => CharacterDataa.Instance;
    internal static SexSystem SexSystem;
    internal static SexMoveExtended LastMove;
    internal static IList<SexTag> AllSexTags;
    #endregion

    internal static void Load(PluginConfig config) {
        try {
            Enabled = config.Entry(nameof(SexChoiceRealismMod), nameof(Enabled), false, 
                "Activates the modification", new PluginConfig.AcceptableValueList<bool>([true, false]));
            RandomPersonalityAlly = config.Entry(nameof(SexChoiceRealismMod), nameof(RandomPersonalityAlly), false,
                "Player companions will take random sex personality", new PluginConfig.AcceptableValueList<bool>([true, false]));
            RandomPersonalityElite = config.Entry(nameof(SexChoiceRealismMod), nameof(RandomPersonalityElite), false,
                "Elite enemies will take random sex personality", new PluginConfig.AcceptableValueList<bool>([true, false]));
            RandomPersonalityEnemy = config.Entry(nameof(SexChoiceRealismMod), nameof(RandomPersonalityEnemy), false,
                "Non-Elite enemies will take random sex personality", new PluginConfig.AcceptableValueList<bool>([true, false]));
            UpdateMoves = config.Entry(nameof(SexChoiceRealismMod), nameof(UpdateMoves), false, 
                "Update SexMove.json", new PluginConfig.AcceptableValueList<bool>([true, false]));
            UsePlayerPreferredPositions = config.Entry(nameof(SexChoiceRealismMod), nameof(UsePlayerPreferredPositions), false, 
                "Player will use ONLY preferred positions", new PluginConfig.AcceptableValueList<bool>([true, false]));

            AllSexTags = (IList<SexTag>)Enum.GetValues(typeof(SexTag));
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
            } else if (UpdateMoves.Value) {
                List<SexMoveExtended> poses = GetSexMoves();
                List<int> newPoses = [];
                foreach (var item in poses) {
                    var move = extendedSexMoves.FirstOrDefault(p => p.ID == item.ID);
                    if (move is null) {
                        if (!extendedSexMoves.Contains(item)) {
                            extendedSexMoves.Add(item);
                            newPoses.Add(item.ID);
                        } else {
                            move.Update(item);
                        }
                    }
                }

                extendedSexMoves.Sort();

                if (JsonUtils.TrySerialize(Plugin.PluginResources, "SexMoves.json", extendedSexMoves)) {
                    Plugin.Log.Info($"SexMoves.json was updated in {Plugin.PluginResources}");
                    File.WriteAllText($"{Plugin.PluginResources}/KnownIds.txt", string.Join(", ", extendedSexMoves.Select(m => m.ID)));
                    File.WriteAllText($"{Plugin.PluginResources}/NewIds.txt", string.Join(", ", newPoses));
                } else {
                    Plugin.Log.Info($"SexMoves.json was not updated");
                }
                UpdateMoves.Value = false;
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

    internal static int GetSexId(CharacterSex casterSex, CharacterSex targetSex) => GetSexId(casterSex, targetSex, null, SexTag.None);
    internal static int GetSexId(CharacterSex casterSex, CharacterSex targetSex, CharacterSex assistSex) => GetSexId(casterSex, targetSex, assistSex, SexTag.None, PositionActionMode.None);
    internal static int GetSexId(CharacterSex casterSex, CharacterSex targetSex, SexTag prefferSexTag) => GetSexId(casterSex, targetSex, null, prefferSexTag, PositionActionMode.None);
    internal static int GetSexId(CharacterSex casterSex, CharacterSex targetSex, PositionActionMode prefferPositionAction) => GetSexId(casterSex, targetSex, null, SexTag.None, prefferPositionAction);
    internal static int GetSexId(CharacterSex casterSex, CharacterSex targetSex, CharacterSex assistSex = null, SexTag prefferSexTag = SexTag.None, PositionActionMode prefferPositionAction = PositionActionMode.None) {
        if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex < 2) {
            Plugin.Log.Info("Exit due execute condition");
            return -1;
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

        var positionGroup = GetPositionGroup(casterSex, targetSex);
        if (positionGroup is PositionGroup.Foreplay) {
            if (casterSex.IsActive && !casterSex.IsFuta && !casterSex.IsMale) {
                Plugin.Log.Debug($"{casterSex.characterName}: Reset Caster role for Foreplay");
                casterSex.IsActive = false;
            }

            if (targetSex.IsActive && !targetSex.IsFuta && !targetSex.IsMale) {
                Plugin.Log.Debug($"{casterSex.characterName}: Reset Target role for Foreplay");
                targetSex.IsActive = false;
            }

            if (assistSex is not null) {
                assistSex.IsActive = casterSex.IsActive;
            }
        }

        List<(SexMoveExtended move, int score)> sexMoves = GetCharacterSexMoves( positionGroup, casterSex, targetSex, assistSex, prefferSexTag );

        if (sexMoves.Count <= 0) {
            Plugin.Log.Info("Exit due EMPTY character sexMoves");
            return -1;
        }

        var move = ChooseWeightedRandom(sexMoves);
        LastMove = move;

        Plugin.Log.Info($"SexID: {move?.ID ?? -1}; Caster role: {(casterSex.IsActive ? "Active" : "Passive")}; Target role: {(targetSex.IsActive ? "Active" : "Passive")}");
        return move is not null ? move.ID : -1;
    }

    internal static void ResetSexCount( CharacterAttributes characterAttributes ) {
        if (characterAttributes.gameObject.TryGetComponentWithCast(out GameplayModComponent modComponent)) {
            modComponent.CumsCount = 0;
            modComponent.SexCount = 0;
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
            Perform = new PersonalitySexTags.SexTags {
                Modifier = 1,
                PreferredTags = SexTag.Universal | SexTag.Sensual,
                NeutralTags = SexTag.Service | SexTag.Wresting,
                AvoidTags = SexTag.Rough | SexTag.Smothering | SexTag.Spanking
            },
            Command = new PersonalitySexTags.SexTags {
                Modifier = 1,
                PreferredTags = SexTag.Sensual | SexTag.Universal,
                NeutralTags = SexTag.Dominant | SexTag.Service,
                AvoidTags = SexTag.Rough | SexTag.Smothering | SexTag.Spanking
            }
        });
        personalitySexTypes.Add(new PersonalitySexTags() {
            Id = 2,
            Name = "Dominant",
            Perform = new PersonalitySexTags.SexTags {
                Modifier = 3,
                PreferredTags = SexTag.Dominant | SexTag.Rough | SexTag.Smothering | SexTag.Spanking,
                NeutralTags = SexTag.Universal | SexTag.Wresting,
                AvoidTags = SexTag.Service | SexTag.Sensual
            },
            Command = new PersonalitySexTags.SexTags {
                Modifier = 1,
                PreferredTags = SexTag.Wresting,
                NeutralTags = SexTag.Rough | SexTag.Sensual | SexTag.Universal,
                AvoidTags = SexTag.Smothering | SexTag.Spanking | SexTag.Service
            }
        });
        personalitySexTypes.Add(new PersonalitySexTags() {
            Id = 3,
            Name = "Defensive",
            Perform = new PersonalitySexTags.SexTags {
                Modifier = 1,
                PreferredTags = SexTag.Sensual | SexTag.Service,
                NeutralTags = SexTag.Universal,
                AvoidTags = SexTag.Dominant | SexTag.Rough | SexTag.Spanking | SexTag.Smothering | SexTag.Wresting
            },
            Command = new PersonalitySexTags.SexTags {
                Modifier = 2,
                PreferredTags = SexTag.Sensual | SexTag.Service,
                NeutralTags = SexTag.Universal,
                AvoidTags = SexTag.Dominant | SexTag.Rough | SexTag.Spanking | SexTag.Smothering
            }
        });
        personalitySexTypes.Add(new PersonalitySexTags() {
            Id = 4,
            Name = "Passionate",
            Perform = new PersonalitySexTags.SexTags {
                Modifier = 2,
                PreferredTags = SexTag.Rough | SexTag.Sensual | SexTag.Wresting,
                NeutralTags = SexTag.Dominant | SexTag.Universal,
                AvoidTags = SexTag.Service
            },
            Command = new PersonalitySexTags.SexTags {
                Modifier = 1,
                PreferredTags = SexTag.Dominant | SexTag.Rough | SexTag.Smothering,
                NeutralTags = SexTag.Wresting | SexTag.Sensual,
                AvoidTags = SexTag.Service
            }
        });
        personalitySexTypes.Add(new PersonalitySexTags() {
            Id = 5,
            Name = "Submissive",
            Perform = new PersonalitySexTags.SexTags {
                Modifier = 1,
                PreferredTags = SexTag.Service,
                NeutralTags = SexTag.Universal | SexTag.Sensual,
                AvoidTags = SexTag.Dominant | SexTag.Rough | SexTag.Spanking | SexTag.Smothering | SexTag.Wresting
            },
            Command = new PersonalitySexTags.SexTags {
                Modifier = 3,
                PreferredTags = SexTag.Dominant | SexTag.Rough | SexTag.Spanking | SexTag.Smothering,
                NeutralTags = SexTag.Sensual,
                AvoidTags = SexTag.Wresting
            }
        });
        personalitySexTypes.Add(new PersonalitySexTags() {
            Id = 6,
            Name = "Trickster",
            Perform = new PersonalitySexTags.SexTags {
                Modifier = 1,
                PreferredTags = SexTag.Rough | SexTag.Smothering | SexTag.Wresting,
                NeutralTags = SexTag.Universal | SexTag.Dominant | SexTag.Sensual,
                AvoidTags = SexTag.Service
            },
            Command = new PersonalitySexTags.SexTags {
                Modifier = 1,
                PreferredTags = SexTag.Dominant | SexTag.Rough | SexTag.Spanking,
                NeutralTags = SexTag.Wresting | SexTag.Sensual,
                AvoidTags = SexTag.Service
            }
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

                if (buffUI.buff.buffName.Contains("Slut collar"))
                    status |= CharacterStatus.SlutCollar;

                if (buffUI.buff.buffName.Contains("Submission collar"))
                    status |= CharacterStatus.SubmissiveCollar;
            }
        }

        return status;
    }

    private static PositionGroup GetPositionGroup(CharacterSex casterSex, CharacterSex targetSex) {
        try {
            int sexChance = 40;
            if (casterSex.gameObject.TryGetComponentWithCast(out GameplayModComponent casterComponent)
                && targetSex.gameObject.TryGetComponentWithCast(out GameplayModComponent targetComponent)) {
                int casterBonus = GetBonus(casterComponent);
                int targetBonus = GetBonus(targetComponent);

                sexChance = Math.Clamp(targetBonus + casterBonus, 5, 95);
            }

            PositionGroup group = RandomUtils.Chance(sexChance) ? PositionGroup.Sex : PositionGroup.Foreplay;
                        
            Plugin.Log.Info($"Select Sex moves: Chance: {sexChance} => Type: {group}");
            return group;

            static int GetBonus(GameplayModComponent component) {
                CharacterStatus status = GetCharacterStatus( component.Attributes);

                int bonus = 0;

                bonus -= component.CumsCount * 5;
                bonus += (component.SexCount - component.CumsCount) * 10;
                bonus += (component.Attributes.currentPleasure <= 0 ? 0 : Math.Min(component.Attributes.currentPleasure / 2500, 1)) * 5;
                bonus += status.HasFlag(CharacterStatus.Aroused) ? 5 : 0;
                bonus += status.HasFlag(CharacterStatus.Charmed) ? 5 : 0;
                bonus += status.HasFlag(CharacterStatus.SlutCollar) ? 5 : 0;
                bonus += status.HasFlag(CharacterStatus.SubmissiveCollar) ? 5 : 0;

                return Math.Max(bonus, 0);
            }
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
            return PositionGroup.Any;
        }
    }

    private static List<(SexMoveExtended move, int score)> GetCharacterSexMoves(PositionGroup positionGroup, CharacterSex casterSex, CharacterSex targetSex, CharacterSex assistSex, SexTag prefferSexTag = SexTag.None, PositionActionMode prefferPositionAction = PositionActionMode.None) {
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
        
        int personalityId = casterSex.gameObject.TryGetComponentWithCast(out GameplayModComponent casterComponent)
            ? casterComponent.PersonalityId
            : 0;
        PersonalitySexTags personality = PersonalitySexTypes.FirstOrDefault( t => t.Id == personalityId )?.UpdateByStatus(GetCharacterStatus(casterSex.characterAttributes));

        CharacterStatus targetStatus = GetCharacterStatus(targetSex.characterAttributes);
        if ( targetStatus is CharacterStatus.Charmed ) {
            prefferSexTag = SexTag.None;
            prefferPositionAction = PositionActionMode.Command;
        }

        foreach (var sexMove in SexMoves) {
            if (sexMove.IsDisabled)
                continue;

            if (!positionGroup.HasFlag(sexMove.PositionGroup))
                continue;

            if (sexMove.ID == casterSex.currentSexEncounter?.SexID)
                continue;

            if (sexMove.IsThreesome != isThreesome)
                continue;

            if (!CheckMainRolesAndGenders(sexMove, casterGender, casterRole, targetGender, targetRole))
                continue;

            if (sexMove.IsThreesome && ((sexMove.AssistGender & assistGender) == 0 || (sexMove.AssistRole & assistRole) == 0 ))
                continue;

            int score = 0;
            if (casterSex.IsPlayer && UsePlayerPreferredPositions.Value) {
                if (!Character.statusDATA.PreferredSex.Contains(sexMove.ID))
                    continue;

                score = 1;
            } else {
                score = GetPositionScore(sexMove, personality, prefferSexTag, prefferPositionAction);
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
        if ((sexMove.CasterGender & casterGender) != 0 && (sexMove.CasterRole & casterRole) != 0
            && (sexMove.TargetGender & targetGender) != 0 && (sexMove.TargetRole & targetRole) != 0) {
            return true;
        }

        // Реверс только если поза универсальная
        if (sexMove.IsCommand && sexMove.IsPerform) {
            if ((sexMove.CasterGender & targetGender) != 0 && (sexMove.CasterRole & targetRole) != 0
                && (sexMove.TargetGender & casterGender) != 0 && (sexMove.TargetRole & casterRole) != 0) {
                return true;
            }
        }

        return false;
    }

    private static int GetPositionScore(SexMoveExtended sexMove, PersonalitySexTags personality, SexTag prefferSexTag = SexTag.None, PositionActionMode prefferPositionAction = PositionActionMode.None) {
        if (personality == null)
            return 1;

        int score = 0;
        if (sexMove.IsPerform) {
            score += GetWeight(sexMove.SexTags, personality.Perform);
        }

        if (sexMove.IsCommand) {
            score += GetWeight(sexMove.SexTags, personality.Command);
        }

        if (prefferSexTag is not SexTag.None) {
            if ((sexMove.SexTags & prefferSexTag) == prefferSexTag)
                score *= 4;
            else if ((sexMove.SexTags & prefferSexTag) != 0)
                score *= 2;
        }

        if (prefferPositionAction is not PositionActionMode.None) {
            if ((sexMove.PositionAction & prefferPositionAction) == prefferPositionAction)
                score *= 4;
            else if ((sexMove.PositionAction & prefferPositionAction) != 0)
                score *= 2;
        }

        /*// SexTag moveTags = sexMove.SexTags;
        //int tagsCount = 0;
        //while (moveTags != 0) {
        //    SexTag tag = moveTags & (SexTag)(-(int)moveTags);

        //    if (sexMove.IsPerform) {
        //        score += GetWeight(tag, personality.Perform);
        //    }

        //    if (sexMove.IsCommand) {
        //        score += GetWeight(tag, personality.Command);
        //    }

        //    moveTags &= moveTags - 1;
        //    tagsCount++;
        //}

        //if (tagsCount > 0)
        //    score /= tagsCount;*/

        int modifier;
        if (sexMove.IsCommand && sexMove.IsPerform) {
            modifier = Math.Max(personality.Perform.Modifier, personality.Command.Modifier);
        } else if (sexMove.IsCommand) {
            modifier = personality.Command.Modifier;
        } else {
            modifier = personality.Perform.Modifier;
        }

        score *= Math.Max(modifier, 1);

        return score == 0 ? 1 : Math.Max(score, 0);

        static int GetWeight(SexTag tags, PersonalitySexTags.SexTags personalityTags) {
            int preferredScore  = 5;
            int neutralScore    = 2;
            int avoidScore      = 3;
            
            int prefferedCount  = BitOperations.PopCount((uint)(tags & personalityTags.PreferredTags));
            int neutralCount    = BitOperations.PopCount((uint)(tags & personalityTags.NeutralTags));
            int avoidCount      = BitOperations.PopCount((uint)(tags & personalityTags.AvoidTags));
            int totalCount      = prefferedCount + neutralCount + avoidCount;

            int weight = 0;
            weight += prefferedCount * RandomUtils.Int32(preferredScore - 1, preferredScore + 1);
            weight += neutralCount * RandomUtils.Int32(neutralScore - 1, neutralScore + 1);
            weight -= avoidCount * RandomUtils.Int32(avoidScore - 1, avoidScore + 1);

            if (totalCount > 0)
                weight /= totalCount;

            return weight;
        }
    }

    private static SexMoveExtended ChooseWeightedRandom(List<(SexMoveExtended move, int score)> moves) {
        int totalWeight = moves.Sum(m => m.score);
        if (totalWeight == 0)
            return null;

        int roll = UnityEngine.Random.Range(0, totalWeight);
        int current = 0;

        foreach (var (move, score) in moves) {
            if (score <= 0)
                continue;

            current += score;
            if (roll < current)
                return move;
        }

        return null;
    }
}
