using Il2Cpp;

namespace YC.GameplayMod.Extensions;
internal static class CombatActionExtensions {
    internal static void ToDefence( this CombatAction action ) {
        action.actionTarget = 0;
        action.actionType = 0;
        action.target = action.caster;
    }
}
