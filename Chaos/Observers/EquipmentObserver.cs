using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Observers.Interfaces;

namespace Chaos.Observers;

public class EquipmentObserver : IPanelObserver<Item>
{
    private readonly Aisling Aisling;

    public EquipmentObserver(Aisling aisling) => Aisling = aisling;

    public void OnAdded(Item obj)
    {
        Aisling.Client.SendEquipment(obj);
        Aisling.Display();
        Aisling.StatSheet.AddWeight(obj.Template.Weight);

        if (obj.Template.Modifiers != null)
            Aisling.StatSheet.Add(obj.Template.Modifiers);

        Aisling.Client.SendAttributes(StatUpdateType.Full);
    }

    public void OnRemoved(byte slot, Item obj)
    {
        Aisling.Client.SendUnequip((EquipmentSlot)slot);
        Aisling.Display();
        Aisling.Client.SendSelfProfile();

        Aisling.StatSheet.AddWeight(-obj.Template.Weight);

        if (obj.Template.Modifiers != null)
            Aisling.StatSheet.Subtract(obj.Template.Modifiers);

        Aisling.Client.SendAttributes(StatUpdateType.Full);
    }

    public void OnUpdated(byte originalSlot, Item obj)
    {
        //uhhhhh nothing for now i guess
    }
}