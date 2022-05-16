using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record LocationSerializer : ServerPacketSerializer<LocationArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Location;
    public override void Serialize(ref SpanWriter writer, LocationArgs args) => writer.WritePoint16(args.Point);
}