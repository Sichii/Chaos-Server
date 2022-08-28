using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record LocationSerializer : ServerPacketSerializer<LocationArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Location;
    public override void Serialize(ref SpanWriter writer, LocationArgs args) => writer.WritePoint16((ushort)args.X, (ushort)args.Y);
}