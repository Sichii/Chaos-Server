using Chaos.Networking.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record LoginControlArgs : ISendArgs
{
    public LoginControlsType LoginControlsType { get; set; }
    public string Message { get; set; } = null!;
}