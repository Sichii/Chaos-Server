#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
#endregion

namespace Chaos.Collections.Abstractions;

/// <summary>
///     Provides methods for managing equipment
/// </summary>
public interface IEquipment : IPanel<Item>
{
    /// <summary>
    ///     Gets the item equipped in the specified slot
    /// </summary>
    Item? this[EquipmentSlot slot] { get; }

    /// <summary>
    ///     Attempts to equip the specified item based on the given equipment type
    /// </summary>
    /// <param name="equipmentType">
    ///     The equipment type of the item
    /// </param>
    /// <param name="item">
    ///     The item to be equipped
    /// </param>
    /// <param name="returnedItem">
    ///     The item that was previously equipped in the slot, if any
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the item is successfully equipped, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryEquip(EquipmentType equipmentType, Item item, out Item? returnedItem);
}