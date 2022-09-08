using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record ServerTableArgs : ISendArgs
{
    public byte[] ServerTable { get; set; } = null!;
}