using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record HeartBeatResponseArgs : ISendArgs
{
    public byte First { get; set; }
    public byte Second { get; set; }
}