using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="WhisperArgs" />
/// </summary>
public sealed record WhisperDeserializer : ClientPacketDeserializer<WhisperArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.Whisper;

    /// <inheritdoc />
    public override WhisperArgs Deserialize(ref SpanReader reader)
    {
        var targetName = reader.ReadString8();
        var message = reader.ReadString8();

        return new WhisperArgs(targetName, message);
    }
}