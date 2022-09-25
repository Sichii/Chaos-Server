using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record SynchronizeTicksResponseArgs : ISendArgs
{
    public int Ticks { get; set; }
}