using Il2Cpp;

namespace YC.Unloader.Models;
internal class CombatActionItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int ActionTarget { get; set; }
    public int ActionType { get; set; }

    public bool IsAttack { get; set; }
    public bool IsSpell { get; set; }
    public bool IsBasicAction { get; set; }
    public bool IsAbility { get; set; }
    public bool IsErotic { get; set; }
    public bool IsDivine { get; set; }
    public bool IsNature { get; set; }
    public bool IsFlame { get; set; }
    public bool IsExplosive { get; set; }
    public bool IsRubyflame { get; set; }
    public bool IsAcidic { get; set; }
    public bool IsWeave { get; set; }
    public bool IsHex { get; set; }
    public bool IsStorm { get; set; }
    public bool IsBolt { get; set; }
    public bool IsArcane { get; set; }
    public bool IsEnhacement { get; set; }
    public bool IsDragon { get; set; }
    public bool IsRestraint { get; set; }
    public bool IsGrapple { get; set; }
    public bool IsWeaponAttack { get; set; }
    public bool IsOneHanded { get; set; }
    public bool IsTwoHanded { get; set; }
    public bool IsUnarmed { get; set; }
    public bool IsKick { get; set; }
    public bool IsWrestling { get; set; }
    public bool IsDominate { get; set; }
    public bool IsCommit { get; set; }
    public bool IsBuff { get; set; }
    public bool IsReversal { get; set; }
    public bool IsStruggle { get; set; }
    public bool IsEscape { get; set; }
    public bool IsStatus { get; set; }

    internal static CombatActionItem FromCombatTalent(CombatAction item) => new CombatActionItem
    {
        Name = item.actionName,
        Description = item.actionDescription,
        IsAttack = item.isAttack,
        IsSpell = item.isSpell,
        IsBasicAction = item.isBasicAction,
        IsAbility = item.isAbility,
        IsErotic = item.isErotic,
        IsDivine = item.isDivine,
        IsNature = item.isNature,
        IsFlame = item.isFlame,
        IsExplosive = item.isExplosive,
        IsRubyflame = item.isRubyflame,
        IsAcidic = item.isAcidic,
        IsWeave = item.isWeave,
        IsHex = item.isHex,
        IsStorm = item.isStorm,
        IsBolt = item.isBolt,
        IsArcane = item.isArcane,
        IsEnhacement = item.isEnhacement,
        IsDragon = item.isDragon,
        IsRestraint = item.isRestraint,
        IsGrapple = item.isGrapple,
        IsWeaponAttack = item.isWeaponAttack,
        IsOneHanded = item.isOneHanded,
        IsTwoHanded = item.isTwoHanded,
        IsUnarmed = item.isUnarmed,
        IsKick = item.isKick,
        IsWrestling = item.isWrestling,
        IsDominate = item.isDominate,
        IsCommit = item.isCommit,
        IsBuff = item.isBuff,
        IsReversal = item.isReversal,
        IsStruggle = item.isStruggle,
        IsEscape = item.isEscape,
        IsStatus = item.isStatus,
        ActionTarget = item.actionTarget,
        ActionType = item.actionType,
    };
}
