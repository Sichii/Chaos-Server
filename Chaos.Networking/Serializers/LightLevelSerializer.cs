using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public sealed record LightLevelSerializer : ServerPacketSerializer<LightLevelArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.LightLevel;

    public override void Serialize(ref SpanWriter writer, LightLevelArgs args) => writer.WriteByte((byte)args.LightLevel);
    //writer.WriteByte(1);
}