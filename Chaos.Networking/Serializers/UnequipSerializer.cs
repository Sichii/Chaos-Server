using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record UnequipSerializer : ServerPacketSerializer<UnequipArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Unequip;
    public override void Serialize(ref SpanWriter writer, UnequipArgs args) => writer.WriteByte((byte)args.EquipmentSlot);
}