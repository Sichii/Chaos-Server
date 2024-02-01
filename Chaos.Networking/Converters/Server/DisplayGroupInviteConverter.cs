using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="DisplayGroupInviteArgs" />
/// </summary>
public sealed class DisplayGroupInviteConverter : PacketConverterBase<DisplayGroupInviteArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.DisplayGroupInvite;

    /// <inheritdoc />
    public override DisplayGroupInviteArgs Deserialize(ref SpanReader reader)
    {
        var groupRequestType = reader.ReadByte();
        var sourceName = reader.ReadString8();

        return new DisplayGroupInviteArgs
        {
            GroupRequestType = (GroupRequestType)groupRequestType,
            SourceName = sourceName
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayGroupInviteArgs args)
    {
        writer.WriteByte((byte)args.GroupRequestType);
        writer.WriteString8(args.SourceName);
    }
}