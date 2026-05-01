using Il2Cpp;

namespace YC.Unloader.Models;
internal class CombatBuffItem
{
    public string Name { get; set; }
    public string Description { get; set; }

    internal static CombatBuffItem FromCombatBuff(CombatBuff buff) => new()
    {
        Name = buff.buffName,
        Description = buff.buffDescription,
    };
}
