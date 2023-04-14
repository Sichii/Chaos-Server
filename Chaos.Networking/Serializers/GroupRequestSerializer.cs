using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="GroupRequestArgs" /> into a buffer
/// </summary>
public sealed record GroupRequestSerializer : ServerPacketSerializer<GroupRequestArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.GroupRequest;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, GroupRequestArgs args)
    {
        writer.WriteByte((byte)args.GroupRequestType);
        writer.WriteString8(args.SourceName);
    }
}