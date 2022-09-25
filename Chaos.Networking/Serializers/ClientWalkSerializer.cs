using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Networking.Extensions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public record ConfirmClientWalkSerializer : ServerPacketSerializer<ConfirmClientWalkArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.ConfirmClientWalk;

    public override void Serialize(ref SpanWriter writer, ConfirmClientWalkArgs args)
    {
        writer.WriteBytes((byte)args.Direction);
        writer.WritePoint16(args.OldPoint);

        //nfi
        writer.WriteUInt16(11);
        writer.WriteUInt16(11);
        writer.WriteByte(1);
    }
}