using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record MapDataArgs : ISendArgs
{
    public byte CurrentYIndex { get; set; }
    public byte[] MapData { get; set; } = Array.Empty<byte>();
    public byte Width { get; set; }
}