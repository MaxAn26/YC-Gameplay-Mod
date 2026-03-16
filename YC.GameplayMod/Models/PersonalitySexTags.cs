using System;

namespace YC.GameplayMod.Models;
internal class PersonalitySexTags {
    public int Id { get; set; }
    public string Name { get; set; }
    public PersonalitySexTags.SexTags Command { get; set; } = new();
    public PersonalitySexTags.SexTags Perform { get; set; } = new();

    internal PersonalitySexTags UpdateByStatus(CharacterStatus status) {
        PersonalitySexTags personality = new () {
            Id = Id,
            Name = Name,
            Command = Command.Copy(),
            Perform = Perform.Copy()
        };

        if (status.HasFlag(CharacterStatus.Collared)) {
            SexTag flags = SexTag.Dominant | SexTag.Rough | SexTag.Wresting | SexTag.Spanking;

            personality.Perform.PreferredTags   &= ~flags;
            personality.Perform.NeutralTags     &= ~flags;

            personality.Command.PreferredTags   |= flags;
            personality.Command.NeutralTags     |= flags;
        }

        if (status.HasFlag(CharacterStatus.Aroused)) {
            personality.Perform.PreferredTags   |= personality.Perform.NeutralTags;
            personality.Perform.NeutralTags     = SexTag.None;

            personality.Command.PreferredTags   |= personality.Command.NeutralTags;
            personality.Command.NeutralTags     = SexTag.None;
        } 
        
        if (status.HasFlag(CharacterStatus.Charmed)) {
            personality.Perform.PreferredTags   |= SexTag.Sensual | SexTag.Service;
            personality.Perform.NeutralTags     &= ~SexTag.Sensual | SexTag.Service;

            personality.Command.PreferredTags   |= SexTag.Sensual | SexTag.Service;
            personality.Command.NeutralTags     &= ~SexTag.Sensual | SexTag.Service;
        }

        if (status.HasFlag(CharacterStatus.SlutCollar)) {
            personality.Perform.PreferredTags   = SexTag.None;
            personality.Perform.NeutralTags     = SexTag.None;
            personality.Perform.AvoidTags       = SexTag.None;

            personality.Command.PreferredTags   = SexTag.Dominant | SexTag.Rough;
            personality.Command.NeutralTags     = SexTag.Sensual | SexTag.Spanking | SexTag.Smothering;
            personality.Command.AvoidTags       = SexTag.None;
        }

        if (status.HasFlag(CharacterStatus.SubmissiveCollar)) {
            personality.Perform.PreferredTags = SexTag.Service;
            personality.Perform.NeutralTags = SexTag.None;
            personality.Perform.AvoidTags = SexTag.None;

            personality.Command.PreferredTags = SexTag.Dominant | SexTag.Rough | SexTag.Spanking | SexTag.Smothering;
            personality.Command.NeutralTags = SexTag.Sensual;
            personality.Command.AvoidTags = SexTag.None;
        }

        return personality;
    }

    public class SexTags {
        public SexTag PreferredTags { get; set; }
        public SexTag NeutralTags { get; set; }
        public SexTag AvoidTags { get; set; }
        public int Modifier { get; set; } = 1;

        internal SexTags Copy() {
            return new SexTags() {
                PreferredTags = PreferredTags,
                NeutralTags = NeutralTags,
                AvoidTags = AvoidTags
            };
        }
    }
}

[Flags]
internal enum CharacterStatus {
    None = 0,
    Collared = 1 << 0,
    Aroused = 1 << 1,
    Charmed = 1 << 2,
    SlutCollar = 1 << 3,
    SubmissiveCollar = 1 << 4,
}
