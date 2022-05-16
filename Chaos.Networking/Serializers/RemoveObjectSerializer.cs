using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record RemoveObjectSerializer : ServerPacketSerializer<RemoveObjectArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveObject;
    public override void Serialize(ref SpanWriter writer, RemoveObjectArgs args) => writer.WriteUInt32(args.SourceId);
}