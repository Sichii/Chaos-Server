using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record LocationArgs : ISendArgs
{
    public int X { get; set; }
    public int Y { get; set; }
}