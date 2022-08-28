using Chaos.Common.Definitions;
using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record SocialStatusDeserializer : ClientPacketDeserializer<SocialStatusArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.SocialStatus;

    public override SocialStatusArgs Deserialize(ref SpanReader reader)
    {
        var socialStatus = (SocialStatus)reader.ReadByte();

        return new SocialStatusArgs(socialStatus);
    }
}