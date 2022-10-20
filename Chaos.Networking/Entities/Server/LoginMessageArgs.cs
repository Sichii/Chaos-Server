using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record LoginMessageArgs : ISendArgs
{
    public LoginMessageType LoginMessageType { get; set; }
    public string? Message { get; set; }
}