using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record MapDataArgs : ISendArgs
{
    public byte CurrentYIndex { get; set; }
    public byte[] MapData { get; set; } = Array.Empty<byte>();
    public byte Width { get; set; }
}