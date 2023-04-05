using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="DisplayChantArgs" />
/// </summary>
public sealed record ChantDeserializer : ClientPacketDeserializer<DisplayChantArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.Chant;

    /// <inheritdoc />
    public override DisplayChantArgs Deserialize(ref SpanReader reader)
    {
        var chantMessage = reader.ReadString8();

        return new DisplayChantArgs(chantMessage);
    }
}