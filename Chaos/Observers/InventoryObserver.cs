using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Observers.Interfaces;

namespace Chaos.Observers;

public class InventoryObserver : IPanelObserver<Item>
{
    private readonly Aisling Aisling;

    public InventoryObserver(Aisling aisling) => Aisling = aisling;

    public void OnAdded(Item obj)
    {
        Aisling.Client.SendAddItemToPane(obj);
        Aisling.StatSheet.AddWeight(obj.Template.Weight);
        Aisling.Client.SendAttributes(StatUpdateType.Primary);
    }

    public void OnRemoved(byte slot, Item obj)
    {
        Aisling.Client.SendRemoveItemFromPane(slot);
        Aisling.StatSheet.AddWeight(-obj.Template.Weight);
        Aisling.Client.SendAttributes(StatUpdateType.Primary);
    }

    public void OnUpdated(byte originalSlot, Item obj) => Aisling.Client.SendAddItemToPane(obj);
}