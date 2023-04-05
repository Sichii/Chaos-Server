using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="BeginChantArgs" />
/// </summary>
public sealed record BeginChantDeserializer : ClientPacketDeserializer<BeginChantArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.BeginChant;

    /// <inheritdoc />
    public override BeginChantArgs Deserialize(ref SpanReader reader)
    {
        var castLineCount = reader.ReadByte();

        return new BeginChantArgs(castLineCount);
    }
}