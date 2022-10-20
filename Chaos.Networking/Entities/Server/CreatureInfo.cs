using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

public sealed record CreatureInfo : VisibleObjectInfo
{
    public CreatureType CreatureType { get; set; }
    public Direction Direction { get; set; }
    public EntityType EntityType { get; set; }
    public string Name { get; set; } = null!;
}