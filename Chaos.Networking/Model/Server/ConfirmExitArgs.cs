using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record ConfirmExitArgs : ISendArgs
{
    public bool ExitConfirmed { get; set; }
}