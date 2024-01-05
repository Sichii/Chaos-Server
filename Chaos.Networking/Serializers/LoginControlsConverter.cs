using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="LoginControlArgs" /> into a buffer
/// </summary>
public sealed class LoginControlsConverter : PacketConverterBase<LoginControlArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.LoginControls;

    /// <inheritdoc />
    public override LoginControlArgs Deserialize(ref SpanReader reader)
    {
        var loginControlsType = reader.ReadByte();
        var message = reader.ReadString8();

        return new LoginControlArgs
        {
            LoginControlsType = (LoginControlsType)loginControlsType,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, LoginControlArgs args)
    {
        writer.WriteByte((byte)args.LoginControlsType);
        writer.WriteString8(args.Message);
    }
}