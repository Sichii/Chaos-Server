using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record AddItemToPaneArgs : ISendArgs
{
    public ItemInfo Item { get; set; } = null!;
}