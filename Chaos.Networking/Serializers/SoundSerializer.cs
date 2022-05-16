using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record SoundSerializer : ServerPacketSerializer<SoundArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Sound;

    public override void Serialize(ref SpanWriter writer, SoundArgs args)
    {
        if (args.IsMusic)
            writer.WriteByte(byte.MaxValue);

        writer.WriteByte(args.Sound);

        if (args.IsMusic)
            writer.WriteBytes(new byte[2]);
    }
}