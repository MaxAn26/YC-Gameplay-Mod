using Il2Cpp;

namespace YC.Unloader.Models;
internal class ClothesItem : IComparable<ClothesItem>
{
    public int ID { get; set; }
    public string Name { get; set; }
    public bool ForMale { get; set; }
    public string Description { get; set; }
    public bool IsArmor { get; set; }
    public bool IsSpecificClothes { get; set; }

    public int Slot { get; set; }

    internal static ClothesItem FromClothes(Clothing clothing) => new()
    {
        ID = clothing.ID,
        Name = clothing.Name,
        Description = clothing.Description,
        IsArmor = clothing.IsArmor,
        IsSpecificClothes = clothing.IsSpecificClothType,
        ForMale = clothing.MaleItem,
        Slot = clothing.Slot
    };

    public int CompareTo(ClothesItem other) => other is null ? 1 : ID.CompareTo(other.ID);

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is not ClothesItem clothingItem2)
        {
            return false;
        }

        return ID == clothingItem2.ID;
    }

    public override int GetHashCode() => ID.GetHashCode();
}
