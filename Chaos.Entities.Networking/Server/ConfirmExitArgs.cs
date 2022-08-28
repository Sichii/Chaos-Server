using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record ConfirmExitArgs : ISendArgs
{
    public bool ExitConfirmed { get; set; }
}