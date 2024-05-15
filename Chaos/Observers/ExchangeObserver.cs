using Chaos.Models.Panel;
using Chaos.Models.World;

namespace Chaos.Observers;

public sealed class ExchangeObserver(Aisling owner, Aisling other) : Abstractions.IObserver<Item>
{
    private readonly Aisling Other = other;
    private readonly Aisling Owner = owner;

    /// <inheritdoc />
    public bool Equals(Abstractions.IObserver<Item>? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return other is ExchangeObserver equipmentObserver
               && Other.Equals(equipmentObserver.Other)
               && Owner.Equals(equipmentObserver.Owner);
    }

    public void OnAdded(Item obj)
    {
        Owner.Client.SendExchangeAddItem(false, obj.Slot, obj);
        Other.Client.SendExchangeAddItem(true, obj.Slot, obj);
    }

    public void OnRemoved(byte slot, Item obj)
    {
        //nothing, this is impossible atm
    }

    public void OnUpdated(byte originalSlot, Item obj)
    {
        Owner.Client.SendExchangeAddItem(false, obj.Slot, obj);
        Other.Client.SendExchangeAddItem(true, obj.Slot, obj);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is ExchangeObserver other && Equals(other));

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Other, Owner, typeof(ExchangeObserver));

    public static bool operator ==(ExchangeObserver? left, ExchangeObserver? right) => Equals(left, right);
    public static bool operator !=(ExchangeObserver? left, ExchangeObserver? right) => !Equals(left, right);
}