using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

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