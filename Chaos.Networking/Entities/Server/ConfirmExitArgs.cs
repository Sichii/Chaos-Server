using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record ConfirmExitArgs : ISendArgs
{
    public bool ExitConfirmed { get; set; }
}