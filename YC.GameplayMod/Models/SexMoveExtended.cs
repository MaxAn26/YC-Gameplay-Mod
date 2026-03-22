using System;
using System.Text.Json.Serialization;

using Il2Cpp;

namespace YC.GameplayMod.Models;
internal class SexMoveExtended : IComparable<SexMoveExtended> {
    private CharacterGender _assistGender = CharacterGender.Any;
    private CharacterRole _assistRole = CharacterRole.Any;

    public bool IsDisabled { get; set; }
    public int Type { get; set; }
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    [JsonPropertyName("AssistGender")]
    public CharacterGender? AssistGenderJson {
        get => IsThreesome ? _assistGender : null;
        set { 
            if (value is not null) 
                _assistGender = value.Value; 
        }
    }

    [JsonPropertyName("AssistRole")]
    public CharacterRole? AssistRoleJson {
        get => IsThreesome ? _assistRole : null;
        set {
            if (value is not null)
                _assistRole = value.Value;
        }
    }

    public CharacterGender CasterGender { get; set; } = CharacterGender.Any;
    public CharacterRole CasterRole { get; set; } = CharacterRole.Any;
    public CharacterGender TargetGender { get; set; } = CharacterGender.Any;
    public CharacterRole TargetRole { get; set; } = CharacterRole.Any;
    public bool IsCommand { get; set; }
    public bool IsPerform { get; set; }
    public SexTag SexTags { get; set; }

    [JsonIgnore]
    public CharacterGender AssistGender { get; set; } = CharacterGender.Any;
    [JsonIgnore]
    public CharacterRole AssistRole { get; set; } = CharacterRole.Any;

    [JsonIgnore]
    internal bool IsThreesome => ID >= 1000;

    [JsonIgnore]
    internal PositionGroup PositionGroup => (IsThreesome && ID is >= 1000 and < 1600) || (!IsThreesome && Type is >= 1 and <= 5) ? PositionGroup.Foreplay : PositionGroup.Sex;

    [JsonIgnore]
    internal PositionActionMode PositionAction { 
        get {
            PositionActionMode mode = PositionActionMode.None;
            if (IsPerform)
                mode |= PositionActionMode.Perform;
            if (IsCommand)
                mode |= PositionActionMode.Command;

            if (mode is PositionActionMode.None)
                mode = PositionActionMode.Perform;

            return mode;
        } }

    public int CompareTo(SexMoveExtended other) => other is null ? 1 : ID.CompareTo(other.ID);

    public override bool Equals(object obj) {
        if (obj is null)
            return false;

        if (obj is not SexMoveExtended sexMove2)
            return false;

        return ID == sexMove2.ID;
    }

    public override int GetHashCode() => ID.GetHashCode();

    internal void Update(SexMoveExtended sexMove) {
        Description = sexMove.Description;
        IsCommand = sexMove.IsCommand;
        IsPerform = sexMove.IsPerform;
        SexTags |= sexMove.SexTags;
    }

    internal static SexMoveExtended FromSexMove(SexMove sexMove) {
        var sexMoveExtended = new SexMoveExtended {
            Type = sexMove.Type,
            ID = sexMove.ID,
            Name = sexMove.Name,
            Description = sexMove.Description,
            IsCommand = sexMove.isCommand,
            IsPerform = sexMove.isPerform,
            SexTags = SexTag.None
        };

        if (sexMove.ID >= 1000)
            sexMoveExtended.Type = 9;

        if (sexMove.TagDominant)
            sexMoveExtended.SexTags |= SexTag.Dominant;
        if (sexMove.TagGeneric)
            sexMoveExtended.SexTags |= SexTag.Generic;
        if (sexMove.TagRough)
            sexMoveExtended.SexTags |= SexTag.Rough;
        if (sexMove.TagSensual)
            sexMoveExtended.SexTags |= SexTag.Sensual;
        if (sexMove.TagService)
            sexMoveExtended.SexTags |= SexTag.Service;
        if (sexMove.TagSmothering)
            sexMoveExtended.SexTags |= SexTag.Smothering;
        if (sexMove.TagSpanking)
            sexMoveExtended.SexTags |= SexTag.Spanking;
        if (sexMove.TagUniversal)
            sexMoveExtended.SexTags |= SexTag.Universal;
        if (sexMove.TagWresting)
            sexMoveExtended.SexTags |= SexTag.Wresting;

        if (sexMove.TagFemdom == sexMove.TagMaledom) {
            sexMoveExtended.CasterGender = CharacterGender.Any;
            sexMoveExtended.CasterRole = CharacterRole.Any;
        } else if (sexMove.TagFemdom) {
            sexMoveExtended.CasterGender = CharacterGender.Female | CharacterGender.Futa;
            sexMoveExtended.CasterRole = CharacterRole.Passive;
        } else if (sexMove.TagMaledom) {
            sexMoveExtended.CasterGender = CharacterGender.Male | CharacterGender.Futa;
            sexMoveExtended.CasterRole = CharacterRole.Active;
        } else {
            sexMoveExtended.CasterGender = CharacterGender.Any;
            sexMoveExtended.CasterRole = CharacterRole.Any;
        }

        return sexMoveExtended;
    }
}

[Flags]
internal enum SexTag {
    None        = 0,
    Dominant    = 1 << 0,
    Generic     = 1 << 1,
    Rough       = 1 << 2,
    Sensual     = 1 << 3,
    Service     = 1 << 4,
    Smothering  = 1 << 5,
    Spanking    = 1 << 6,
    Universal   = 1 << 7,
    Wresting    = 1 << 8
}

[Flags]
internal enum PositionGroup {
    None        = 0,
    Foreplay    = 1 << 0,
    Sex         = 1 << 1,
    Any         = Foreplay | Sex,
}

[Flags]
internal enum PositionActionMode {
    None        = 0,
    Command     = 1 << 0,
    Perform     = 1 << 1,
    Any         = Command | Perform,
}

[Flags]
internal enum CharacterGender {
    None        = 0,
    Female      = 1 << 0,
    Futa        = 1 << 1,
    Male        = 1 << 2,
    Any         = Female | Futa | Male
}

[Flags]
internal enum CharacterRole {
    None        = 0,
    Active      = 1 << 0,
    Passive     = 1 << 1,
    Any         = Active | Passive
}
