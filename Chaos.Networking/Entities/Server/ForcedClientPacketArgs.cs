using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

public sealed record ForcedClientPacketArgs : ISendArgs
{
    public ClientOpCode ClientOpCode { get; set; }
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public byte[] Data { get; set; } = Array.Empty<byte>();
}