using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

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