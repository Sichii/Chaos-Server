using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record SynchronizeTicksResponseArgs : ISendArgs
{
    public int Ticks { get; set; }
}