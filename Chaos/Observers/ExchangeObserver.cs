using Chaos.Objects.Panel;
using Chaos.Objects.World;

namespace Chaos.Observers;

public class ExchangeObserver : Abstractions.IObserver<Item>
{
    private readonly Aisling Other;
    private readonly Aisling Owner;

    public ExchangeObserver(Aisling owner, Aisling other)
    {
        Owner = owner;
        Other = other;
    }

    public void OnAdded(Item obj)
    {
        Owner.Client.SendExchangeAddItem(false, obj.Slot, obj);
        Other.Client.SendExchangeAddItem(true, obj.Slot, obj);
    }

    public void OnRemoved(byte slot, Item obj)
    {
        //nothing
    }

    public void OnUpdated(byte originalSlot, Item obj)
    {
        Owner.Client.SendExchangeAddItem(false, obj.Slot, obj);
        Other.Client.SendExchangeAddItem(true, obj.Slot, obj);
    }
}