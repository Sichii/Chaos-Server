using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record NoticeRequestArgs : ISendArgs
{
    public bool IsFullResponse { get; set; }
    public byte[]? Notification { get; set; }
    public uint? NotificationCheckSum { get; set; }
}