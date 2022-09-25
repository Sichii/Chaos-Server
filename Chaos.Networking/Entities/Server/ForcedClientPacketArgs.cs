using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

public record ForcedClientPacketArgs : ISendArgs
{
    public ClientOpCode ClientOpCode { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
}