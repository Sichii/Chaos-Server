using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Networking.Extensions;
using Chaos.Packets.Abstractions;

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