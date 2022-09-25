using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record CreatureTurnArgs : ISendArgs
{
    public Direction Direction { get; set; }
    public uint SourceId { get; set; }
}