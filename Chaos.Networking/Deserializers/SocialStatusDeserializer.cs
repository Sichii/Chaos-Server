using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Model.Client;
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