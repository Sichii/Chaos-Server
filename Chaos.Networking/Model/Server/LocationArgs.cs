using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record LocationArgs : ISendArgs
{
    public int X { get; set; }
    public int Y { get; set; }
}