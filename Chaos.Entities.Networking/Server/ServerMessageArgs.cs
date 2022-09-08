using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record ServerMessageArgs : ISendArgs
{
    public string Message { get; set; } = null!;
    public ServerMessageType ServerMessageType { get; set; }
}