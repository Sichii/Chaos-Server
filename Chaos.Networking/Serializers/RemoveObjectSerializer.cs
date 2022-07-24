using Chaos.IO.Memory;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record RemoveObjectSerializer : ServerPacketSerializer<RemoveObjectArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveObject;
    public override void Serialize(ref SpanWriter writer, RemoveObjectArgs args) => writer.WriteUInt32(args.SourceId);
}