using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="EmoteArgs" />
/// </summary>
public sealed class EmoteConverter : PacketConverterBase<EmoteArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Emote;

    /// <inheritdoc />
    public override EmoteArgs Deserialize(ref SpanReader reader)
    {
        var bodyAnimation = reader.ReadByte();

        return new EmoteArgs
        {
            BodyAnimation = (BodyAnimation)(bodyAnimation + 9)
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, EmoteArgs args) => writer.WriteByte((byte)(args.BodyAnimation - 9));
}