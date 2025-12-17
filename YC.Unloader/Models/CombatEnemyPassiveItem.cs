using Il2Cpp;

namespace YC.Unloader.Models;
internal class CombatEnemyPassiveItem {
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsEffect1InPercent { get; set; }
    public int Effect1Type { get; set; }
    public int Effect1Value { get; set; }
    public bool IsEffect2InPercent { get; set; }
    public int Effect2Type { get; set; }
    public int Effect2Value { get; set; }
    public bool IsElite { get; set; }
    internal static CombatEnemyPassiveItem FromCombatTalent(CombatEnemyPassive item) {
        return new CombatEnemyPassiveItem {
            Name = item.passiveName,
            Description = item.passiveDescription,
            IsEffect1InPercent = item.Effect1IsPercent,
            Effect1Type = item.Effect1Type,
            Effect1Value = item.Effect1Value,
            IsEffect2InPercent = item.Effect2IsPercent,
            Effect2Type = item.Effect2Type,
            Effect2Value = item.Effect2Value,
            IsElite = item.isElite,
        };
    }
}
