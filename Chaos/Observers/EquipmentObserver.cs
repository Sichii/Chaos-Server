using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Observers.Abstractions;

namespace Chaos.Observers;

public class EquipmentObserver : IPanelObserver<Item>
{
    private readonly Aisling Aisling;

    public EquipmentObserver(Aisling aisling) => Aisling = aisling;

    public void OnAdded(Item obj)
    {
        Aisling.Client.SendEquipment(obj);
        Aisling.Display();
        Aisling.UserStatSheet.AddWeight(obj.Template.Weight);

        if (obj.Template.Modifiers != null)
            Aisling.UserStatSheet.Add(obj.Template.Modifiers);

        Aisling.Client.SendAttributes(StatUpdateType.Full);
    }

    public void OnRemoved(byte slot, Item obj)
    {
        Aisling.Client.SendUnequip((EquipmentSlot)slot);
        Aisling.Display();
        Aisling.Client.SendSelfProfile();

        Aisling.UserStatSheet.AddWeight(-obj.Template.Weight);

        if (obj.Template.Modifiers != null)
            Aisling.UserStatSheet.Subtract(obj.Template.Modifiers);

        Aisling.Client.SendAttributes(StatUpdateType.Full);
    }

    public void OnUpdated(byte originalSlot, Item obj)
    {
        //uhhhhh nothing for now i guess
    }
}