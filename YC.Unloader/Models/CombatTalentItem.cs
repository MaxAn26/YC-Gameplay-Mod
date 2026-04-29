using System;
using System.Collections.Generic;

using Il2Cpp;

namespace YC.Unloader.Models;
internal class CombatTalentItem : IComparable<CombatTalentItem>
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description1 { get; set; }
    public string Description2 { get; set; } = string.Empty;
    public string Description3 { get; set; } = string.Empty;
    public bool IsCombatTalent { get; set; }
    public List<int> BannedTalents { get; set; } = [];
    public List<int> RequiredTalents { get; set; } = [];

    public int CompareTo(CombatTalentItem other) => ID.CompareTo(other.ID);

    internal static CombatTalentItem FromCombatTalent(CombatTalent item) => new CombatTalentItem
    {
        ID = item.ID,
        Name = item.talentName,
        Description1 = item.talentDescriptionLevel1,
        Description2 = item.talentDescriptionLevel2,
        Description3 = item.talentDescriptionLevel3,
        IsCombatTalent = item.isCombatTalent,
        BannedTalents = [.. item.bannedTalents],
        RequiredTalents = [.. item.requiredTalents],
    };
}
