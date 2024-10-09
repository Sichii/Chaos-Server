using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;

namespace Chaos.Observers;

public sealed class EquipmentObserver(Aisling aisling) : Abstractions.IObserver<Item>
{
    private readonly Aisling Aisling = aisling;

    /// <inheritdoc />
    public bool Equals(Abstractions.IObserver<Item>? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return other is EquipmentObserver equipmentObserver && Aisling.Equals(equipmentObserver.Aisling);
    }

    public void OnAdded(Item obj)
    {
        Aisling.Client.SendEquipment(obj);
        Aisling.Display();
        Aisling.UserStatSheet.AddWeight(obj.Weight);

        if (!obj.Modifiers.Equals(default))
            Aisling.UserStatSheet.AddBonus(obj.Modifiers);

        Aisling.Client.SendAttributes(StatUpdateType.Full);
        obj.Script.OnEquipped(Aisling);
    }

    public void OnRemoved(byte slot, Item obj)
    {
        Aisling.Client.SendDisplayUnequip((EquipmentSlot)slot);
        Aisling.Display();
        Aisling.Client.SendSelfProfile();

        Aisling.UserStatSheet.AddWeight(-obj.Weight);

        if (!obj.Modifiers.Equals(default))
            Aisling.UserStatSheet.SubtractBonus(obj.Modifiers);

        Aisling.Client.SendAttributes(StatUpdateType.Full);
        obj.Script.OnUnEquipped(Aisling);
    }

    public void OnUpdated(byte originalSlot, Item obj)
    {
        //uhhhhh nothing for now i guess
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is EquipmentObserver other && Equals(other));

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Aisling, typeof(EquipmentObserver));

    public static bool operator ==(EquipmentObserver? left, EquipmentObserver? right) => Equals(left, right);
    public static bool operator !=(EquipmentObserver? left, EquipmentObserver? right) => !Equals(left, right);
}