using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="LoginControlArgs" />
/// </summary>
public sealed class LoginControlConverter : PacketConverterBase<LoginControlArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.LoginControl;

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