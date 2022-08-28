using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

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