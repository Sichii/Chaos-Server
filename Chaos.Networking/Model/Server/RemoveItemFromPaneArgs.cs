using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record RemoveItemFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}