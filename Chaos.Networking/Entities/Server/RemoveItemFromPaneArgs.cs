using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record RemoveItemFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}