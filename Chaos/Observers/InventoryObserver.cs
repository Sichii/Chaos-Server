using Chaos.Common.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;

namespace Chaos.Observers;

public sealed class InventoryObserver(Aisling aisling) : Abstractions.IObserver<Item>
{
    private readonly Aisling Aisling = aisling;

    /// <inheritdoc />
    public bool Equals(Abstractions.IObserver<Item>? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return other is InventoryObserver equipmentObserver && Aisling.Equals(equipmentObserver.Aisling);
    }

    public void OnAdded(Item obj)
    {
        Aisling.Client.SendAddItemToPane(obj);
        Aisling.UserStatSheet.AddWeight(obj.Weight);
        Aisling.Client.SendAttributes(StatUpdateType.Primary);
    }

    public void OnRemoved(byte slot, Item obj)
    {
        Aisling.Client.SendRemoveItemFromPane(slot);
        Aisling.UserStatSheet.AddWeight(-obj.Weight);
        Aisling.Client.SendAttributes(StatUpdateType.Primary);
    }

    public void OnUpdated(byte originalSlot, Item obj) => Aisling.Client.SendAddItemToPane(obj);

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is InventoryObserver other && Equals(other));

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Aisling, typeof(InventoryObserver));

    public static bool operator ==(InventoryObserver? left, InventoryObserver? right) => Equals(left, right);
    public static bool operator !=(InventoryObserver? left, InventoryObserver? right) => !Equals(left, right);
}