using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="SendPublicMessageArgs" />
/// </summary>
public sealed class PublicMessageConverter : PacketConverterBase<SendPublicMessageArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.SendPublicMessage;

    /// <inheritdoc />
    public override SendPublicMessageArgs Deserialize(ref SpanReader reader)
    {
        var publicMessageType = reader.ReadByte();
        var message = reader.ReadString8();

        return new SendPublicMessageArgs
        {
            PublicMessageType = (PublicMessageType)publicMessageType,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SendPublicMessageArgs args)
    {
        writer.WriteByte((byte)args.PublicMessageType);
        writer.WriteString8(args.Message);
    }
}