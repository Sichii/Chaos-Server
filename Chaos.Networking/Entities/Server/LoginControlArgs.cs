using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record LoginControlArgs : ISendArgs
{
    public LoginControlsType LoginControlsType { get; set; }
    public string Message { get; set; } = null!;
}