using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="DisplayPublicMessageArgs" /> into a buffer
/// </summary>
public sealed class DisplayPublicMessageConverter : PacketConverterBase<DisplayPublicMessageArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.DisplayPublicMessage;

    /// <inheritdoc />
    public override DisplayPublicMessageArgs Deserialize(ref SpanReader reader)
    {
        var messageType = reader.ReadByte();
        var sourceId = reader.ReadUInt32();
        var message = reader.ReadString8();

        return new DisplayPublicMessageArgs
        {
            PublicMessageType = (PublicMessageType)messageType,
            SourceId = sourceId,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayPublicMessageArgs args)
    {
        writer.WriteByte((byte)args.PublicMessageType);
        writer.WriteUInt32(args.SourceId);
        writer.WriteString8(args.Message);
    }
}