using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="LoginNoticeArgs" /> into a buffer
/// </summary>
public sealed class LoginNoticeConverter : PacketConverterBase<LoginNoticeArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.LoginNotice;

    /// <inheritdoc />
    public override LoginNoticeArgs Deserialize(ref SpanReader reader)
    {
        var isFullResponse = reader.ReadBoolean();

        var args = new LoginNoticeArgs
        {
            IsFullResponse = isFullResponse
        };

        if (isFullResponse)
        {
            var data = reader.ReadData16();

            args.Data = data;
        } else
        {
            var checkSum = reader.ReadUInt32();

            args.CheckSum = checkSum;
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, LoginNoticeArgs args)
    {
        writer.WriteBoolean(args.IsFullResponse);

        if (args.IsFullResponse)
            writer.WriteData16(args.Data!);
        else
            writer.WriteUInt32(args.CheckSum!.Value);
    }
}