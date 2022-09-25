using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public record EffectSerializer : ServerPacketSerializer<EffectArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Effect;

    public override void Serialize(ref SpanWriter writer, EffectArgs args)
    {
        writer.WriteUInt16(args.EffectIcon);
        writer.WriteByte((byte)args.EffectColor);
    }
}