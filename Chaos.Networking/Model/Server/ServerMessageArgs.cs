using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record ServerMessageArgs : ISendArgs
{
    public string Message { get; set; } = null!;
    public ServerMessageType ServerMessageType { get; set; }
}