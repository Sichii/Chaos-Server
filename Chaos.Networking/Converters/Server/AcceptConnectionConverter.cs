using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="AcceptConnectionArgs" /> into a buffer
/// </summary>
public class AcceptConnectionConverter : PacketConverterBase<AcceptConnectionArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.AcceptConnection;

    /// <inheritdoc />
    public override AcceptConnectionArgs Deserialize(ref SpanReader reader)
    {
        _ = reader.ReadByte();
        var message = reader.ReadString();

        return new AcceptConnectionArgs
        {
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, AcceptConnectionArgs args)
    {
        writer.WriteByte(27);
        writer.WriteString(args.Message, true);
    }
}