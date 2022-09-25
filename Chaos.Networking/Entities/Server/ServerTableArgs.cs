using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record ServerTableArgs : ISendArgs
{
    public byte[] ServerTable { get; set; } = null!;
}