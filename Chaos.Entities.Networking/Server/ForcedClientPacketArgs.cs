using Chaos.Packets.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record ForcedClientPacketArgs : ISendArgs
{
    public ClientOpCode ClientOpCode { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
}