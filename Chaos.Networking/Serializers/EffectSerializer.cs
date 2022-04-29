using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

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