using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="TurnArgs" />
/// </summary>
public sealed record TurnDeserializer : ClientPacketDeserializer<TurnArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.Turn;

    /// <inheritdoc />
    public override TurnArgs Deserialize(ref SpanReader reader)
    {
        var direction = (Direction)reader.ReadByte();

        return new TurnArgs(direction);
    }
}