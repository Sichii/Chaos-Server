using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

namespace Chaos.Networking.Deserializers;

public record PublicMessageDeserializer : ClientPacketDeserializer<PublicMessageArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.PublicMessage;

    public override PublicMessageArgs Deserialize(ref SpanReader reader)
    {
        var publicMessageType = (PublicMessageType)reader.ReadByte();
        var message = reader.ReadString8();

        return new PublicMessageArgs(publicMessageType, message);
    }
}