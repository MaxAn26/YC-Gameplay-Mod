using Il2Cpp;

namespace YC.GameplayMod.Extensions;
internal static class CharacterSexExtensions
{
    internal static bool IsCharmed(this CharacterSex characterSex) => characterSex.charmedEx > 0 || characterSex.characterAttributes?.CheckForStatus("Charmed") == true;
}
