using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="PasswordChangeArgs" />
/// </summary>
public sealed class PasswordChangeConverter : PacketConverterBase<PasswordChangeArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.PasswordChange;

    /// <inheritdoc />
    public override PasswordChangeArgs Deserialize(ref SpanReader reader)
    {
        var name = reader.ReadString8();
        var currentPw = reader.ReadString8();
        var newPw = reader.ReadString8();

        return new PasswordChangeArgs
        {
            Name = name,
            CurrentPassword = currentPw,
            NewPassword = newPw
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, PasswordChangeArgs args)
    {
        writer.WriteString8(args.Name);
        writer.WriteString8(args.CurrentPassword);
        writer.WriteString8(args.NewPassword);
    }
}