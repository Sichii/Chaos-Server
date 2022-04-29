using Chaos.Core.Definitions;
using Chaos.Observers.Interfaces;
using Chaos.PanelObjects;
using Chaos.WorldObjects;

namespace Chaos.Observers;

public class WeightObserver : IPanelObserver<Item>
{
    private readonly object Sync;
    private readonly User User;

    public WeightObserver(User user)
    {
        User = user;
        Sync = new object();
    }

    public void OnAdded(Item obj)
    {
        lock (Sync)
        {
            User.StatSheet.CurrentWeight += obj.Weight;
            User.Client.SendAttributes(StatUpdateType.Primary);
        }
    }

    public void OnRemoved(byte slot, Item obj)
    {
        lock (Sync)
        {
            User.StatSheet.CurrentWeight -= obj.Weight;
            User.Client.SendAttributes(StatUpdateType.Primary);
        }
    }

    public void OnUpdated(byte originalSlot, Item obj)
    {
        //nothing
    }
}