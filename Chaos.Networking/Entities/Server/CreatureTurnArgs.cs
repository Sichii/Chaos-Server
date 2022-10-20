using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record CreatureTurnArgs : ISendArgs
{
    public Direction Direction { get; set; }
    public uint SourceId { get; set; }
}