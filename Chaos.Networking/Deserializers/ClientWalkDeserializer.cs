using Chaos.Entities.Networking.Client;
using Chaos.Geometry.Definitions;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

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