using Chaos.Geometry.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Model.Client;
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