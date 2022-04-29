using Chaos.Core.Definitions;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Definitions;

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