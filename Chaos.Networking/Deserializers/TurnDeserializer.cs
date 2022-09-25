using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public record TurnDeserializer : ClientPacketDeserializer<TurnArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Turn;

    public override TurnArgs Deserialize(ref SpanReader reader)
    {
        var direction = (Direction)reader.ReadByte();

        return new TurnArgs(direction);
    }
}