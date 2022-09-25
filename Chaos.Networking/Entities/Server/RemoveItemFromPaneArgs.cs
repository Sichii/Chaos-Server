using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record RemoveItemFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}