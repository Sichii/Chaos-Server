using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public record ForcedClientPacketSerializer : ServerPacketSerializer<ForcedClientPacketArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.ForceClientPacket;

    public override void Serialize(ref SpanWriter writer, ForcedClientPacketArgs args)
    {
        writer.WriteUInt16((ushort)(args.Data.Length + 1));
        writer.WriteByte((byte)args.ClientOpCode);
        writer.WriteData(args.Data);
    }
}