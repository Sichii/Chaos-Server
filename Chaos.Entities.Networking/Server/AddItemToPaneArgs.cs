using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record AddItemToPaneArgs : ISendArgs
{
    public ItemInfo Item { get; set; } = null!;
}