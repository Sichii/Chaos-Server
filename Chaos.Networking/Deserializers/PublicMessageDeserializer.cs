using Chaos.Core.Definitions;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Definitions;

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