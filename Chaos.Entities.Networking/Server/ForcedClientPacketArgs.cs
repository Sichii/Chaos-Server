using Chaos.Packets.Abstractions;
using Chaos.Packets.Definitions;

namespace Chaos.Entities.Networking.Server;

public record ForcedClientPacketArgs : ISendArgs
{
    public ClientOpCode ClientOpCode { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
}