using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record AddItemToPaneArgs : ISendArgs
{
    public ItemArg Item { get; set; } = null!;
}