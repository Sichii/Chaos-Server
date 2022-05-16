using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record GroupRequestSerializer : ServerPacketSerializer<GroupInviteArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.GroupRequest;

    public override void Serialize(ref SpanWriter writer, GroupInviteArgs args)
    {
        writer.WriteByte((byte)args.GroupRequestType);
        writer.WriteString8(args.SourceName);
    }
}