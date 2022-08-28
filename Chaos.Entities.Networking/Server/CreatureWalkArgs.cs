using Chaos.Geometry.Definitions;
using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record CreatureWalkArgs : ISendArgs
{
    public Direction Direction { get; set; }
    public IPoint OldPoint { get; set; } = null!;
    public uint SourceId { get; set; }
}