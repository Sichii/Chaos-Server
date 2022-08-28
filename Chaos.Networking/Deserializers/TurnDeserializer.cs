using Chaos.Entities.Networking.Client;
using Chaos.Geometry.Definitions;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

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