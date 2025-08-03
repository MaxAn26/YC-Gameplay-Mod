using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BaseMod.Core.Extensions;
using BaseMod.Core.Utils;

using BepInEx.Configuration;

using Il2CppInterop.Runtime;

using SexInteractionMod.Models;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace YC.GameplayMod.Mods;
internal class SexChoiceRealismMod {
    #region Configuration
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<bool> UpdateMoves;
    internal static ConfigEntry<bool> UsePlayerPreferredPositions;
    #endregion

    #region States
    internal static bool IsModActive => Enabled.Value;
    internal static List<SexMoveExtended> SexMoves { get; set; } = [];
    #endregion

    #region Storage
    internal static CharacterDataa Character => CharacterDataa.Instance;
    internal static SexSystem SexSystem;
    internal static int LastSexType = 0;
    #endregion

    internal static void Load(ConfigFile config) {
        try {
            Enabled = config.Bind(nameof(SexChoiceRealismMod), nameof(Enabled), false,
                new ConfigDescription("Activates the modification", new AcceptableValueList<bool>([true, false])));
            UpdateMoves = config.Bind(nameof(SexChoiceRealismMod), nameof(UpdateMoves), false,
                new ConfigDescription("Update SexMove.json", new AcceptableValueList<bool>([true, false])));
            UsePlayerPreferredPositions = config.Bind(nameof(SexChoiceRealismMod), nameof(UsePlayerPreferredPositions), false,
                new ConfigDescription("ONLY use preferred positions", new AcceptableValueList<bool>([true, false])));

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
        } catch (Exception ex) {
            Plugin.Log.Error(ex.Message);
        }
    }

    internal static void SetSexID() {
        try {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex < 2) {
                Plugin.Log.Info("Exit due execute condition");
                return;
            }

            if (SexSystem is null) {
                Plugin.Log.Info("Exit due SexSystem is NULL");
                return;
            }

            if (SexMoves.Count == 0) {
                Plugin.Log.Info("Exit due EMPTY SexPositions");
                return;
            }

            var move = GetSexMove();
            if (move is null) {
                Plugin.Log.Info("Exit due SexMove is null");
                return;
            }

            Plugin.Log.Info($"Set SexMove: ID: {move.ID}({move.Type}) Name: '{move.Name}'");
            SexSystem.SexType = move.Type;
            SexSystem.SexID = move.ID;
        } catch (Exception ex) {
            Plugin.Log.Error(ex);
        }
    }

    internal static void SetThreesomeSexID() {
        try {
            if (!Enabled.Value || SceneManager.GetActiveScene().buildIndex < 2) {
                Plugin.Log.Info("Exit due execute condition");
                return;
            }

            if (SexSystem is null) {
                Plugin.Log.Info("Exit due SexSystem is NULL");
                return;
            }

            if (SexMoves.Count == 0) {
                Plugin.Log.Info("Exit due EMPTY SexPositions");
                return;
            }

            var move = GetThreesomeSexMove();
            if (move is null) {
                Plugin.Log.Info("Exit due SexMove is null");
                return;
            }

            Plugin.Log.Info($"Set SexMove: ID: {move.ID}({move.Type}) Name: '{move.Name}'");
            SexSystem.SexType = move.Type;
            SexSystem.SexID = move.ID;
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

    private static SexMoveExtended GetSexMove() {
        List<SexMoveExtended> sexMoves = GetCharacterSexMoves( SexSystem.IsThreesome );

        if (sexMoves.Count == 0)
            return null;

        sexMoves.Shuffle();
        var move = sexMoves.RandomItem();
        LastSexType = move.Type;
        return move;
    }

    private static SexMoveExtended GetThreesomeSexMove() {
        List<SexMoveExtended> sexMoves = GetCharacterSexMoves( SexSystem.IsThreesome ); //[];
        
        if (sexMoves.Count == 0)
            return null;

        sexMoves.Shuffle();
        var move = sexMoves.RandomItem();
        LastSexType = move.Type;
        return move;
    }

    private static List<SexMoveExtended> GetCharacterSexMoves(bool isThreesome) {
        List<SexMoveExtended> sexMoves = [];
        CharacterGender caster = SexSystem.CasterActive ? CharacterGender.Male : CharacterGender.Female;
        CharacterGender target = SexSystem.TargetActive ? CharacterGender.Male : CharacterGender.Female;
        bool targetIsCharmed = SexSystem.Target.GetComponentWithCast<CharacterSex>()?.IsCharmed ?? false;

        foreach (var sexMove in SexMoves) {
            if (sexMove.IsDisabled)
                continue;

            if (isThreesome != sexMove.IsThreesome)
                continue;

            if (sexMove.IsCommand && !targetIsCharmed)
                continue;

            if (SexSystem.PlayerAttacker && UsePlayerPreferredPositions.Value && !Character.statusDATA.PreferredSex.Contains(sexMove.ID))
                continue;

            if (!sexMove.IsUniversal && sexMove.CasterRole is not CharacterRole.Any) {
                if (SexSystem.CasterActive && sexMove.CasterRole is not CharacterRole.Active)
                    continue;
                else if (!SexSystem.CasterActive && sexMove.CasterRole is not CharacterRole.Passive)
                    continue;
                else
                    continue;
            }

            if (sexMove.CasterGender is not CharacterGender.Any && sexMove.CasterGender != caster)
                continue;

            if (sexMove.TargetGender is not CharacterGender.Any && sexMove.TargetGender != target)
                continue;

            sexMoves.Add(sexMove);
        }

        return sexMoves;
    }
}

internal enum SexPositionType {
    All,
    Foreplay,
    Other,
    Sex
}