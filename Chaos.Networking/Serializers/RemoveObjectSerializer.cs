using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record RemoveObjectSerializer : ServerPacketSerializer<RemoveObjectArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveObject;
    public override void Serialize(ref SpanWriter writer, RemoveObjectArgs args) => writer.WriteUInt32(args.SourceId);
}