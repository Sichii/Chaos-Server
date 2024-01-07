using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="LoginMessageArgs" /> into a buffer
/// </summary>
public sealed class LoginMessageConverter : PacketConverterBase<LoginMessageArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.LoginMessage;

    /// <inheritdoc />
    public override LoginMessageArgs Deserialize(ref SpanReader reader)
    {
        var type = reader.ReadByte();
        var message = reader.ReadString8();

        return new LoginMessageArgs
        {
            LoginMessageType = (LoginMessageType)type,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, LoginMessageArgs args)
    {
        writer.WriteByte((byte)args.LoginMessageType);
        writer.WriteString8(args.LoginMessageType == LoginMessageType.Confirm ? "\0" : args.Message!);
    }
}