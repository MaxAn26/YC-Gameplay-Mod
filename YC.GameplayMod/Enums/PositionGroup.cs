namespace YC.GameplayMod.Enums;

[Flags]
internal enum PositionGroup
{
    None = 0,
    Foreplay = 1 << 0,
    Sex = 1 << 1,
    Any = Foreplay | Sex,
}
