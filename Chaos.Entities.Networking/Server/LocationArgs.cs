using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record LocationArgs : ISendArgs
{
    public int X { get; set; }
    public int Y { get; set; }
}