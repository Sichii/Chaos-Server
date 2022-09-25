using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

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