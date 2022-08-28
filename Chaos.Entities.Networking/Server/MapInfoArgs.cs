using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record MapInfoArgs : ISendArgs
{
    public ushort CheckSum { get; set; }
    public byte Flags { get; set; }
    public byte Height { get; set; }
    public short MapId { get; set; }
    public string Name { get; set; } = null!;
    public byte Width { get; set; }
}