using System;

namespace YC.GameplayMod.Models;
internal class PersonalitySexTags {
    public int Id { get; set; }
    public string Name { get; set; }
    public SexTag PreferredTags { get; set; }
    public SexTag NeutralTags { get; set; }
    public SexTag AvoidTags { get; set; }

    internal PersonalitySexTags UpdateByStatus(CharacterStatus status) {
        PersonalitySexTags sexType = new () {
            Id = Id,
            Name = Name,
            PreferredTags = PreferredTags,
            NeutralTags = NeutralTags,
            AvoidTags = AvoidTags
        };

        if (status.HasFlag(CharacterStatus.Collared)) {
            SexTag flagsToRemove = SexTag.Dominant | SexTag.Wresting | SexTag.Spanking;

            sexType.PreferredTags &= ~flagsToRemove;

            sexType.NeutralTags &= ~flagsToRemove;
        } else if (status.HasFlag(CharacterStatus.Aroused)) {
            sexType.PreferredTags |= sexType.NeutralTags;
            sexType.NeutralTags = SexTag.None;
        } else if (status.HasFlag(CharacterStatus.Charmed)) {
            sexType.PreferredTags |= SexTag.Sensual | SexTag.Service;
            sexType.NeutralTags &= ~SexTag.Sensual | SexTag.Service;
        }

        return sexType;
    }
}

[Flags]
internal enum CharacterStatus {
    None = 0,
    Collared = 1 << 0,
    Aroused = 1 << 1,
    Charmed = 1 << 2
}