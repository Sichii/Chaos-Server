using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record HeartBeatResponseArgs : ISendArgs
{
    public byte First { get; set; }
    public byte Second { get; set; }
}