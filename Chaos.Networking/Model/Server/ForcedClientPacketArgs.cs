using Chaos.Packets.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record ForcedClientPacketArgs : ISendArgs
{
    public ClientOpCode ClientOpCode { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
}