using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="EmoteArgs" />
/// </summary>
public sealed record EmoteDeserializer : ClientPacketDeserializer<EmoteArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.Emote;

    /// <inheritdoc />
    public override EmoteArgs Deserialize(ref SpanReader reader)
    {
        var bodyAnimation = (BodyAnimation)(reader.ReadByte() + 9);

        return new EmoteArgs(bodyAnimation);
    }
}