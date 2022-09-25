using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record HeartBeatResponseArgs : ISendArgs
{
    public byte First { get; set; }
    public byte Second { get; set; }
}