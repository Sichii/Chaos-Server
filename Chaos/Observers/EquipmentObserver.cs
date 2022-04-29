using Chaos.Core.Definitions;
using Chaos.Observers.Interfaces;
using Chaos.PanelObjects;
using Chaos.WorldObjects;

namespace Chaos.Observers;

public class EquipmentObserver : IPanelObserver<Item>
{
    private readonly User User;

    public EquipmentObserver(User user) => User = user;

    public void OnAdded(Item obj)
    {
        User.Client.SendEquipment(obj);
        User.MapInstance.ShowUser(User);
        User.Client.SendAttributes(StatUpdateType.Full);
    }

    public void OnRemoved(byte slot, Item obj)
    {
        User.Client.SendSelfProfile();
        User.Client.SendAttributes(StatUpdateType.Full);
        User.Client.SendUnequip((EquipmentSlot)slot);
        User.MapInstance.ShowUser(User);
    }

    public void OnUpdated(byte originalSlot, Item obj)
    {
        //uhhhhh nothing for now i guess
    }
}