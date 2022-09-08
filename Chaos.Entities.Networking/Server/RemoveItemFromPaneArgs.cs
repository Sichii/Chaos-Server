using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record RemoveItemFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}