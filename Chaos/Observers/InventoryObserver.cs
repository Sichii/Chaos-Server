using Chaos.Observers.Interfaces;
using Chaos.PanelObjects;
using Chaos.WorldObjects;

namespace Chaos.Observers;

public class InventoryObserver : IPanelObserver<Item>
{
    private readonly User User;

    public InventoryObserver(User user) => User = user;

    public void OnAdded(Item obj) => User.Client.SendAddItemToPane(obj);

    public void OnRemoved(byte slot, Item obj) => User.Client.SendRemoveItemFromPane(slot);

    public void OnUpdated(byte originalSlot, Item obj) => User.Client.SendAddItemToPane(obj);
}