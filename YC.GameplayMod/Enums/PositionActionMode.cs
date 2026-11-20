namespace YC.GameplayMod.Enums;

[Flags]
internal enum PositionActionMode
{
    None = 0,
    Command = 1 << 0,
    Perform = 1 << 1,
    Any = Command | Perform,
}
