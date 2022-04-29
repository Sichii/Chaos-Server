using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record ConnectionInfoArgs : ISendArgs
{
    public byte[] Key { get; set; } = Array.Empty<byte>();
    public byte Seed { get; set; }
    public uint TableCheckSum { get; set; }
}