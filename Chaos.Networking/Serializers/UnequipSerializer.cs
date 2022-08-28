using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record UnequipSerializer : ServerPacketSerializer<UnequipArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Unequip;
    public override void Serialize(ref SpanWriter writer, UnequipArgs args) => writer.WriteByte((byte)args.EquipmentSlot);
}