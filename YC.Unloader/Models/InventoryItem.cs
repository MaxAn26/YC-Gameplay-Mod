using Il2Cpp;

namespace YC.Unloader.Models;
internal class InventoryItem
{
    public int Id { get; set; }
    public int Type { get; set; }
    public bool IsConsumable { get; set; }
    public bool IsTrinket { get; set; }
    public bool IsWeapon { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public int Quality { get; set; }


    internal static InventoryItem FromCombatConsumable(CombatConsumable item) => new InventoryItem
    {
        Id = item.itemID,
        Type = item.itemType,
        IsConsumable = true,
        IsTrinket = false,
        IsWeapon = false,
        Name = item.itemName,
        Description = item.itemDescription,
        Price = item.itemPrice,
        Quality = item.itemQuality,
    };

    internal static InventoryItem FromCombatItem(CombatItem item) => new InventoryItem
    {
        Id = item.itemID,
        Type = item.itemType,
        IsConsumable = true,
        IsTrinket = false,
        IsWeapon = false,
        Name = item.itemName,
        Description = item.itemDescription,
        Price = item.itemPrice,
        Quality = item.itemQuality,
    };

    internal static InventoryItem FromCombatTrinket(CombatTrinket item) => new InventoryItem
    {
        Id = item.itemID,
        Type = item.itemType,
        IsConsumable = false,
        IsTrinket = true,
        IsWeapon = false,
        Name = item.itemName,
        Description = item.itemDescription,
        Price = item.itemPrice,
        Quality = item.itemQuality,
    };

    internal static InventoryItem FromCombatWeapon(CombatWeapon item) => new InventoryItem
    {
        Id = item.itemID,
        Type = item.itemType,
        IsConsumable = false,
        IsTrinket = false,
        IsWeapon = true,
        Name = item.itemName,
        Description = item.itemDescription,
        Price = item.itemPrice,
        Quality = item.itemQuality,
    };
}
