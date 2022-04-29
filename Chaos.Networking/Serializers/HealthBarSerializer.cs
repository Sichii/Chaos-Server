using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

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