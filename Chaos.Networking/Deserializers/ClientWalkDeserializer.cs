using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="ClientWalkArgs" />
/// </summary>
public sealed record ClientWalkDeserializer : ClientPacketDeserializer<ClientWalkArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.ClientWalk;

    /// <inheritdoc />
    public override ClientWalkArgs Deserialize(ref SpanReader reader)
    {
        var direction = (Direction)reader.ReadByte();
        var stepCount = reader.ReadByte();

        return new ClientWalkArgs(direction, stepCount);
    }
}