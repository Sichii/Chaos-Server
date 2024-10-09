using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="PublicMessageArgs" />
/// </summary>
public sealed class PublicMessageConverter : PacketConverterBase<PublicMessageArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.PublicMessage;

    /// <inheritdoc />
    public override PublicMessageArgs Deserialize(ref SpanReader reader)
    {
        var publicMessageType = reader.ReadByte();
        var message = reader.ReadString8();

        return new PublicMessageArgs
        {
            PublicMessageType = (PublicMessageType)publicMessageType,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, PublicMessageArgs args)
    {
        writer.WriteByte((byte)args.PublicMessageType);
        writer.WriteString8(args.Message);
    }
}