using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record ConnectionInfoArgs : ISendArgs
{
    public byte[] Key { get; set; } = Array.Empty<byte>();
    public byte Seed { get; set; }
    public uint TableCheckSum { get; set; }
}