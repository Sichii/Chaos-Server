using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

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