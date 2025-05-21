#region
using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Models.Panel;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a panel that's used to manage equipped items
/// </summary>
public sealed class Equipment : PanelBase<Item>, IEquipment
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Equipment" /> class.
    /// </summary>
    /// <param name="items">
    ///     The items to populate this panel with
    /// </param>
    public Equipment(IEnumerable<Item>? items = null)
        : base(PanelType.Equipment, 19, [0])
    {
        items ??= [];

        foreach (var item in items)
            Objects[item.Slot] = item;
    }

    /// <summary>
    ///     Gets the item in the specified slot
    /// </summary>
    /// <param name="slot">
    ///     The equipment slot to retreive an item from
    /// </param>
    public Item? this[EquipmentSlot slot] => this[(byte)slot];

    /// <inheritdoc />
    public bool TryEquip(EquipmentType equipmentType, Item item, out Item? returnedItem)
    {
        returnedItem = null;

        if (equipmentType == EquipmentType.NotEquipment)
            throw new InvalidOperationException(
                $"Item {item.DisplayName} ({item.UniqueId}) has equipment type of {EquipmentType.NotEquipment}");

        using var @lock = Sync.EnterScope();

        var possibleSlots = equipmentType.ToEquipmentSlots();
        byte bSlot = 0;

        //check for empty slots
        foreach (var slot in possibleSlots)
        {
            bSlot = (byte)slot;

            if (TryAdd(bSlot, item))
                return true;
        }

        //no slots empty? try to replace
        TryGetRemove(bSlot, out returnedItem);
        TryAdd(bSlot, item);

        return true;
    }
}