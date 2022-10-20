using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record SynchronizeTicksResponseArgs : ISendArgs
{
    public int Ticks { get; set; }
}