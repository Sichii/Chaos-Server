using System.Collections.Immutable;
using Chaos.Common.Definitions;

namespace Chaos.Networking.Definitions;

/// <summary>
///     Represents the constants used by the networking layer
/// </summary>
public static class NETWORKING_CONSTANTS
{
    /// <summary>
    ///     The offset value for a creature's sprite
    /// </summary>
    internal const ushort CREATURE_SPRITE_OFFSET = 16384;
    /// <summary>
    ///     The offset value for an item's sprite
    /// </summary>
    internal const ushort ITEM_SPRITE_OFFSET = 32768;

    /// <summary>
    ///     The order in which equipment slots are sent to the client
    /// </summary>
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