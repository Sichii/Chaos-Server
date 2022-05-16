using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

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