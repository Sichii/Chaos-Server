using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public record LoginNotificationSerializer : ServerPacketSerializer<NoticeRequestArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.LoginNotification;

    public override void Serialize(ref SpanWriter writer, NoticeRequestArgs args)
    {
        writer.WriteBoolean(args.IsFullResponse);

        if (args.IsFullResponse)
            writer.WriteData16(args.Notification!);
        else
            writer.WriteUInt32(args.NotificationCheckSum!.Value);
    }
}