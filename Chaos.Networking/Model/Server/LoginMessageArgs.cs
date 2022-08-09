using Chaos.Networking.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record LoginMessageArgs : ISendArgs
{
    public LoginMessageType LoginMessageType { get; set; }
    public string? Message { get; set; }
}