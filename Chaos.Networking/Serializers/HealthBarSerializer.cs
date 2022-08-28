using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record HealthBarSerializer : ServerPacketSerializer<HealthBarArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.HealthBar;

    public override void Serialize(ref SpanWriter writer, HealthBarArgs args)
    {
        writer.WriteUInt32(args.SourceId);
        writer.WriteByte(0);
        writer.WriteByte(args.HealthPercent);
        writer.WriteByte(args.Sound);
    }
}