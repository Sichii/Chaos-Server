using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Observers.Abstractions;

namespace Chaos.Observers;

public class ExchangeObserver : IPanelObserver<Item>
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