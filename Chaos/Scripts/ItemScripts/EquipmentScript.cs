using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class EquipmentScript : ItemScriptBase
{
    public EquipmentScript(Item item)
        : base(item) { }

    public override void OnUse(Aisling source)
    {
        //gender check
        if ((Source.Template.Gender != source.Gender) && (Source.Template.Gender != Gender.Unisex))
            return;

        var slot = Source.Slot;

        //try equip,
        if (source.Equipment.TryEquip(Source, out var returnedItem))
        {
            source.Inventory.Remove(slot);
            Source.Script.OnEquipped(source);

            if (returnedItem != null)
                source.Inventory.TryAddToNextSlot(returnedItem);
        }
    }
}