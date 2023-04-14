using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="UserIdArgs" /> into a buffer
/// </summary>
public sealed record UserIdSerializer : ServerPacketSerializer<UserIdArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.UserId;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, UserIdArgs args)
    {
        writer.WriteUInt32(args.Id);
        writer.WriteByte((byte)args.Direction);
        writer.WriteByte(0); //dunno
        writer.WriteByte((byte)args.BaseClass);
        writer.WriteByte(0); //dunno
        writer.WriteByte(0); //dunno
        writer.WriteByte(0); //dunno
    }
}