using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Observers.Interfaces;

namespace Chaos.Observers;

public class EquipmentObserver : IPanelObserver<Item>
{
    private readonly User User;

    public EquipmentObserver(User user) => User = user;

    public void OnAdded(Item obj)
    {
        User.Client.SendEquipment(obj);
        User.MapInstance.ShowUser(User);
        User.StatSheet.AddWeight(obj.Template.Weight);

        if (obj.Template.Modifiers != null)
            User.StatSheet.Add(obj.Template.Modifiers);

        User.Client.SendAttributes(StatUpdateType.Full);
    }

    public void OnRemoved(byte slot, Item obj)
    {
        User.Client.SendUnequip((EquipmentSlot)slot);
        User.MapInstance.ShowUser(User);
        User.Client.SendSelfProfile();

        User.StatSheet.AddWeight(-obj.Template.Weight);

        if (obj.Template.Modifiers != null)
            User.StatSheet.Subtract(obj.Template.Modifiers);

        User.Client.SendAttributes(StatUpdateType.Full);
    }

    public void OnUpdated(byte originalSlot, Item obj)
    {
        //uhhhhh nothing for now i guess
    }
}