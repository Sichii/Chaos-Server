using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="GroupRequestArgs" /> into a buffer
/// </summary>
public sealed class GroupRequestConverter : PacketConverterBase<GroupRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.GroupInvite;

    /// <inheritdoc />
    public override GroupRequestArgs Deserialize(ref SpanReader reader)
    {
        var groupRequestType = reader.ReadByte();
        var sourceName = reader.ReadString8();

        return new GroupRequestArgs
        {
            GroupRequestType = (GroupRequestType)groupRequestType,
            SourceName = sourceName
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, GroupRequestArgs args)
    {
        writer.WriteByte((byte)args.GroupRequestType);
        writer.WriteString8(args.SourceName);
    }
}