using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace YC.GameplayMod.Models;
internal class SexMoveExtended : IComparable<SexMoveExtended> {
    public bool IsDisabled { get; set; }
    public int Type { get; set; }
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public CharacterGender CasterGender { get; set; } = CharacterGender.Any;
    public CharacterRole CasterRole { get; set; } = CharacterRole.Any;
    public CharacterGender TargetGender { get; set; } = CharacterGender.Any;
    public CharacterRole TargetRole { get; set; } = CharacterRole.Any;
    public bool IsCommand { get; set; }
    public bool IsPerform { get; set; }
    public bool IsDominant { get; set; }
    public bool IsSensual { get; set; }
    public bool IsService { get; set; }
    public bool IsSmothering { get; set; }
    public bool IsSpanking { get; set; }
    public bool IsUniversal { get; set; }
    public bool IsWresting { get; set; }

    [JsonIgnore]
    public bool IsThreesome => ID >= 1000;
    [JsonIgnore]
    public bool IsForeplay => Type is >= 1 and <= 5;

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
        IsDominant = sexMove.IsDominant;
        IsSensual = sexMove.IsSensual;
        IsService = sexMove.IsService;
        IsSmothering = sexMove.IsSmothering;
        IsSpanking = sexMove.IsSpanking;
        IsUniversal = sexMove.IsUniversal;
        IsWresting = sexMove.IsWresting;
    }

    internal static SexMoveExtended FromSexMove(SexMove sexMove) {
        var sexMoveExtended = new SexMoveExtended() {
            Type = sexMove.Type,
            ID = sexMove.ID,
            Name = sexMove.Name,
            Description = sexMove.Description,
            IsCommand = sexMove.isCommand,
            IsPerform = sexMove.isPerform,
            IsDominant = sexMove.TagDominant,
            IsSensual = sexMove.TagSensual,
            IsService = sexMove.TagService,
            IsSmothering = sexMove.TagSmothering,
            IsSpanking = sexMove.TagSpanking,
            IsUniversal = sexMove.TagUniversal,
            IsWresting = sexMove.TagWresting
        };

        if (sexMove.TagFemdom && !sexMove.TagMaledom)
            sexMoveExtended.CasterRole = CharacterRole.Passive;
        else if (!sexMove.TagFemdom && sexMove.TagMaledom)
            sexMoveExtended.CasterRole = CharacterRole.Active;
        else
            sexMoveExtended.CasterRole = CharacterRole.Any;

        return sexMoveExtended;
    }
}

internal enum CharacterGender {
    [EnumMember]
    Any,
    [EnumMember]
    Female,
    [EnumMember]
    Male
}

internal enum CharacterRole {
    [EnumMember]
    Any,
    [EnumMember]
    Active,
    [EnumMember]
    Passive
}