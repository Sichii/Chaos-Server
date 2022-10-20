using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public sealed record GroupRequestSerializer : ServerPacketSerializer<GroupInviteArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.GroupRequest;

    public override void Serialize(ref SpanWriter writer, GroupInviteArgs args)
    {
        writer.WriteByte((byte)args.GroupRequestType);
        writer.WriteString8(args.SourceName);
    }
}