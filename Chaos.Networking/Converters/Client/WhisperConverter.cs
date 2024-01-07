using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="WhisperArgs" />
/// </summary>
public sealed class WhisperConverter : PacketConverterBase<WhisperArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Whisper;

    /// <inheritdoc />
    public override WhisperArgs Deserialize(ref SpanReader reader)
    {
        var targetName = reader.ReadString8();
        var message = reader.ReadString8();

        return new WhisperArgs
        {
            TargetName = targetName,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, WhisperArgs args)
    {
        writer.WriteString8(args.TargetName);
        writer.WriteString8(args.Message);
    }
}