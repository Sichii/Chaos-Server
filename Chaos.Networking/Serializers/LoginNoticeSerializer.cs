using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="LoginNoticeArgs" /> into a buffer
/// </summary>
public sealed record LoginNoticeSerializer : ServerPacketSerializer<LoginNoticeArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.LoginNotice;

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