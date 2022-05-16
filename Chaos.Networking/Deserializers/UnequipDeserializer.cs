using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

namespace Chaos.Networking.Deserializers;

public record UnequipDeserializer : ClientPacketDeserializer<UnequipArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Unequip;

    public override UnequipArgs Deserialize(ref SpanReader reader)
    {
        var equipmentSlot = (EquipmentSlot)reader.ReadByte();

        return new UnequipArgs(equipmentSlot);
    }
}