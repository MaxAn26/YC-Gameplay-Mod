using Il2Cpp;

namespace YC.GameplayMod.Extensions;
internal static class CharacterAttributeExtensions {
    internal static bool IsCompanion( this CharacterAttributes characterAttributes, bool includePlayer = true ) {
        if (characterAttributes.isPlayer && includePlayer)
            return true;

        if (characterAttributes.combatAI.isAlly)
            return true;
        
        return false;
    }

    internal static bool IsEnemy(this CharacterAttributes characterAttributes) {
        if (characterAttributes.isPlayer || characterAttributes.combatAI.isAlly)
            return false;

        return true;
    }
}
