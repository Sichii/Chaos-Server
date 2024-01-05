using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="ServerMessageArgs" /> into a buffer
/// </summary>
public sealed class ServerMessageConverter : PacketConverterBase<ServerMessageArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.ServerMessage;

    /// <inheritdoc />
    public override ServerMessageArgs Deserialize(ref SpanReader reader)
    {
        var messageType = reader.ReadByte();
        var message = reader.ReadString16();

        return new ServerMessageArgs
        {
            ServerMessageType = (ServerMessageType)messageType,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ServerMessageArgs args)
    {
        writer.WriteByte((byte)args.ServerMessageType);
        writer.WriteString16(args.Message);
    }
}