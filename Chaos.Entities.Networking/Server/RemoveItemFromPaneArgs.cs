using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record RemoveItemFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}