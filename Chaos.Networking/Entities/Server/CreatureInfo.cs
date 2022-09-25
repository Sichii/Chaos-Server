using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

public record CreatureInfo : VisibleObjectInfo
{
    public CreatureType CreatureType { get; set; }
    public Direction Direction { get; set; }
    public GameObjectType GameObjectType { get; set; }
    public string Name { get; set; } = null!;
}