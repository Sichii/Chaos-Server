using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class EquipmentScript : ItemScriptBase
{
    public EquipmentScript(Item item)
        : base(item) { }

    public override void OnUse(Aisling aisling)
    {
        //gender check
        if ((Source.Template.Gender != aisling.Gender) && (Source.Template.Gender != Gender.Unisex))
            return;

        var slot = Source.Slot;

        //try equip,
        if (aisling.Equipment.TryEquip(Source, out var returnedItem))
        {
            aisling.Inventory.Remove(slot);
            Source.Script.OnEquipped(aisling);

            if (returnedItem != null)
                aisling.Inventory.TryAddToNextSlot(returnedItem);
        }
    }
}