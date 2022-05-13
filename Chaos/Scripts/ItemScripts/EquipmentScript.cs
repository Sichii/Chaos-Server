using Chaos.Containers;
using Chaos.Core.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class EquipmentScript : ItemScriptBase
{
    public EquipmentScript(Item item)
        : base(item) { }

    public override void OnUnequip(User user)
    {
        //stuff like.. if the item applies an effect or something, undo it?
    }

    public override void OnUse(User user)
    {
        //gender check
        if ((Source.Template.Gender != user.Gender) && (Source.Template.Gender != Gender.Unisex))
            return;

        var slot = Source.Slot;

        //try equip,
        if (user.Equipment.TryEquip(Source, out var returnedItem))
        {
            user.Inventory.Remove(slot);

            if (returnedItem != null)
                user.Inventory.TryAddToNextSlot(returnedItem);
        }
    }
}