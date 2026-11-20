using System.Text.Json.Serialization;
using Il2Cpp;
using YC.GameplayMod.Enums;

namespace YC.GameplayMod.Models;
internal class SexMoveExtended : IComparable<SexMoveExtended>
{
    private Character _assist = new();

    public bool IsDisabled { get; set; }
    public int Type { get; set; }
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    [JsonPropertyName("Assist")]
    public Character Assist
    {
        get => IsThreesome ? _assist : null;
        set
        {
            if (value is not null)
            {
                _assist = value;
            }
        }
    }

    public Character Caster { get; set; } = new();
    public Character Target { get; set; } = new();
    public bool IsCommand { get; set; }
    public bool IsPerform { get; set; }
    public SexTag SexTags { get; set; }

    [JsonIgnore]
    internal bool IsThreesome => ID >= 1000;

    [JsonIgnore]
    internal PositionGroup PositionGroup => IsThreesome && ID is >= 1000 and < 1600 || !IsThreesome && Type is >= 1 and <= 5 ? PositionGroup.Foreplay : PositionGroup.Sex;

    [JsonIgnore]
    internal PositionActionMode PositionAction
    {
        get
        {
            PositionActionMode mode = PositionActionMode.None;
            if (IsPerform)
            {
                mode |= PositionActionMode.Perform;
            }

            if (IsCommand)
            {
                mode |= PositionActionMode.Command;
            }

            if (mode is PositionActionMode.None)
            {
                mode = PositionActionMode.Perform;
            }

            return mode;
        }
    }

    public int CompareTo(SexMoveExtended other) => other is null ? 1 : ID.CompareTo(other.ID);

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is not SexMoveExtended sexMove2)
        {
            return false;
        }

        return ID == sexMove2.ID;
    }

    public override int GetHashCode() => ID.GetHashCode();

    internal void Update(SexMoveExtended sexMove)
    {
        Description = sexMove.Description;
        IsCommand = sexMove.IsCommand;
        IsPerform = sexMove.IsPerform;
        SexTags |= sexMove.SexTags;
    }

    internal static SexMoveExtended FromSexMove(SexMove sexMove)
    {
        var sexMoveExtended = new SexMoveExtended
        {
            Type = sexMove.Type,
            ID = sexMove.ID,
            Name = sexMove.Name,
            Description = sexMove.Description,
            Caster = new(),
            Target = new(),
            IsCommand = sexMove.isCommand,
            IsPerform = sexMove.isPerform,
            SexTags = SexTag.None
        };

        if (sexMove.ID >= 1000)
        {
            sexMoveExtended.Type = 9;
        }

        if (sexMove.TagDominant)
        {
            sexMoveExtended.SexTags |= SexTag.Dominant;
        }

        if (sexMove.TagGeneric)
        {
            sexMoveExtended.SexTags |= SexTag.Generic;
        }

        if (sexMove.TagRough)
        {
            sexMoveExtended.SexTags |= SexTag.Rough;
        }

        if (sexMove.TagSensual)
        {
            sexMoveExtended.SexTags |= SexTag.Sensual;
        }

        if (sexMove.TagService)
        {
            sexMoveExtended.SexTags |= SexTag.Service;
        }

        if (sexMove.TagSmothering)
        {
            sexMoveExtended.SexTags |= SexTag.Smothering;
        }

        if (sexMove.TagSpanking)
        {
            sexMoveExtended.SexTags |= SexTag.Spanking;
        }

        if (sexMove.TagUniversal)
        {
            sexMoveExtended.SexTags |= SexTag.Universal;
        }

        if (sexMove.TagWresting)
        {
            sexMoveExtended.SexTags |= SexTag.Wresting;
        }

        if (sexMove.TagFemdom == sexMove.TagMaledom)
        {
            sexMoveExtended.Caster.Gender = CharacterGender.Any;
            sexMoveExtended.Caster.Role = CharacterRole.Any;
        }
        else if (sexMove.TagFemdom)
        {
            sexMoveExtended.Caster.Gender = CharacterGender.Female | CharacterGender.Futa;
            sexMoveExtended.Caster.Role = CharacterRole.Passive;
        }
        else if (sexMove.TagMaledom)
        {
            sexMoveExtended.Caster.Gender = CharacterGender.Male | CharacterGender.Futa;
            sexMoveExtended.Caster.Role = CharacterRole.Active;
        }
        else
        {
            sexMoveExtended.Caster.Gender = CharacterGender.Any;
            sexMoveExtended.Caster.Role = CharacterRole.Any;
        }

        return sexMoveExtended;
    }

    internal class Character
    {
        public CharacterCumCondition Cum { get; set; } = CharacterCumCondition.Always;
        public CharacterGender Gender { get; set; } = CharacterGender.Any;
        public CharacterRole Role { get; set; } = CharacterRole.Any;
    }
}
