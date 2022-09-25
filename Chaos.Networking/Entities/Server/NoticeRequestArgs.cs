using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record NoticeRequestArgs : ISendArgs
{
    public bool IsFullResponse { get; set; }
    public byte[]? Notification { get; set; }
    public uint? NotificationCheckSum { get; set; }
}