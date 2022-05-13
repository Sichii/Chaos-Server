using Chaos.Core.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Observers.Interfaces;

namespace Chaos.Observers;

public class InventoryObserver : IPanelObserver<Item>
{
    private readonly User User;

    public InventoryObserver(User user) => User = user;

    public void OnAdded(Item obj)
    {
        User.Client.SendAddItemToPane(obj);
        User.StatSheet.AddWeight(obj.Template.Weight);
        User.Client.SendAttributes(StatUpdateType.Primary);
    }

    public void OnRemoved(byte slot, Item obj)
    {
        User.Client.SendRemoveItemFromPane(slot);
        User.StatSheet.AddWeight(-obj.Template.Weight);
        User.Client.SendAttributes(StatUpdateType.Primary);
    }

    public void OnUpdated(byte originalSlot, Item obj) => User.Client.SendAddItemToPane(obj);
}