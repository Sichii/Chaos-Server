using Chaos.Core.Definitions;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Deserializers;

public record ClientWalkDeserializer : ClientPacketDeserializer<ClientWalkArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.ClientWalk;

    public override ClientWalkArgs Deserialize(ref SpanReader reader)
    {
        var direction = (Direction)reader.ReadByte();
        var stepCount = reader.ReadByte();

        return new ClientWalkArgs(direction, stepCount);
    }
}