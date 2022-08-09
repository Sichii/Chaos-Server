using System.Collections.Immutable;

namespace Chaos.Networking.Definitions;

public static class NETWORKING_CONSTANTS
{
    public const int CREATURE_SPRITE_OFFSET = 16384;
    public const int ITEM_SPRITE_OFFSET = 32768;

    public static readonly ImmutableArray<EquipmentSlot> PROFILE_EQUIPMENTSLOT_ORDER = ImmutableArray.Create(
        EquipmentSlot.Weapon,
        EquipmentSlot.Armor,
        EquipmentSlot.Shield,
        EquipmentSlot.Helmet,
        EquipmentSlot.Earrings,
        EquipmentSlot.Necklace,
        EquipmentSlot.LeftRing,
        EquipmentSlot.RightRing,
        EquipmentSlot.LeftGaunt,
        EquipmentSlot.RightGaunt,
        EquipmentSlot.Belt,
        EquipmentSlot.Greaves,
        EquipmentSlot.Accessory1,
        EquipmentSlot.Boots,
        EquipmentSlot.Overcoat,
        EquipmentSlot.OverHelm,
        EquipmentSlot.Accessory2,
        EquipmentSlot.Accessory3);
}