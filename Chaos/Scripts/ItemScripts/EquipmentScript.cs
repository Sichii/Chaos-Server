using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class EquipmentScript : ItemScriptBase
{
    public EquipmentScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source)
    {
        //gender check
        if ((Subject.Template.Gender != source.Gender) && (Subject.Template.Gender != Gender.Unisex))
            return;

        var slot = Subject.Slot;

        //try equip,
        if (source.Equipment.TryEquip(Subject, out var returnedItem))
        {
            source.Inventory.Remove(slot);
            Subject.Script.OnEquipped(source);

            if (returnedItem != null)
                source.Inventory.TryAddToNextSlot(returnedItem);
        }
    }
}