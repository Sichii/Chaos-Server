using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record ConnectionInfoArgs : ISendArgs
{
    public byte[] Key { get; set; } = Array.Empty<byte>();
    public byte Seed { get; set; }
    public uint TableCheckSum { get; set; }
}