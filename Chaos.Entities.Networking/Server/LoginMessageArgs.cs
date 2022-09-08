using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record LoginMessageArgs : ISendArgs
{
    public LoginMessageType LoginMessageType { get; set; }
    public string? Message { get; set; }
}