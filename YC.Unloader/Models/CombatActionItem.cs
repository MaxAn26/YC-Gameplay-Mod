using Il2Cpp;

namespace YC.Unloader.Models;
internal class CombatActionItem {
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsAbility { get; set; }
    public bool IsAttack { get; set; }
    public bool IsBasicAction { get; set; }
    public bool IsBuff { get; set; }
    public bool IsCommit { get; set; }
    public bool IsDominate { get; set; }
    public bool IsErotic { get; set; }
    public bool IsEscape { get; set; }
    public bool IsGrapple { get; set; }
    public bool IsRestraint { get; set; }
    public bool IsStruggle { get; set; }

    internal static CombatActionItem FromCombatTalent(CombatAction item) {
        return new CombatActionItem {
            Name = item.actionName,
            Description = item.actionDescription,
            IsAbility = item.isAbility,
            IsAttack = item.isAttack,
            IsBuff = item.isBuff,
            IsCommit = item.isCommit,
            IsDominate = item.isDominate,
            IsBasicAction = item.isBasicAction,
            IsErotic = item.isErotic,
            IsEscape = item.isEscape,
            IsGrapple = item.isGrapple,
            IsRestraint = item.isRestraint,
            IsStruggle = item.isStruggle,
        };
    }
}
