using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record CreatureWalkArgs : ISendArgs
{
    public Direction Direction { get; set; }
    public IPoint OldPoint { get; set; } = null!;
    public uint SourceId { get; set; }
}