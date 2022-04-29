using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record ServerTableArgs : ISendArgs
{
    public byte[] ServerTable { get; set; } = null!;
}