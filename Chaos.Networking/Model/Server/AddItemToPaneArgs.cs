using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record AddItemToPaneArgs : ISendArgs
{
    public ItemInfo Item { get; set; } = null!;
}