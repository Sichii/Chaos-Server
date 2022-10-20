using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record MapInfoArgs : ISendArgs
{
    public ushort CheckSum { get; set; }
    public byte Flags { get; set; }
    public byte Height { get; set; }
    public short MapId { get; set; }
    public string Name { get; set; } = null!;
    public byte Width { get; set; }
}