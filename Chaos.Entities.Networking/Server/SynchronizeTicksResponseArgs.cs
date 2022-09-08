using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record SynchronizeTicksResponseArgs : ISendArgs
{
    public int Ticks { get; set; }
}