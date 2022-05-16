using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record CreatureTurnArgs : ISendArgs
{
    public Direction Direction { get; set; }
    public uint SourceId { get; set; }
}