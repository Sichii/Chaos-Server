using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="PublicMessageArgs" /> into a buffer
/// </summary>
public sealed class PublicMessageConverter : PacketConverterBase<PublicMessageArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.PublicMessage;

    /// <inheritdoc />
    public override PublicMessageArgs Deserialize(ref SpanReader reader)
    {
        var messageType = reader.ReadByte();
        var sourceId = reader.ReadUInt32();
        var message = reader.ReadString8();

        return new PublicMessageArgs
        {
            PublicMessageType = (PublicMessageType)messageType,
            SourceId = sourceId,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, PublicMessageArgs args)
    {
        writer.WriteByte((byte)args.PublicMessageType);
        writer.WriteUInt32(args.SourceId);
        writer.WriteString8(args.Message);
    }
}