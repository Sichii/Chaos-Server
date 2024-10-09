using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="EmoteArgs" />
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