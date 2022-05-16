using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record LightLevelSerializer : ServerPacketSerializer<LightLevelArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.LightLevel;

    public override void Serialize(ref SpanWriter writer, LightLevelArgs args) => writer.WriteByte((byte)args.LightLevel);
    //writer.WriteByte(1);
}