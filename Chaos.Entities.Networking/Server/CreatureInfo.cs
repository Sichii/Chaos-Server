using Chaos.Common.Definitions;
using Chaos.Geometry.Definitions;

namespace Chaos.Entities.Networking.Server;

public record CreatureInfo : VisibleObjectInfo
{
    public CreatureType CreatureType { get; set; }
    public Direction Direction { get; set; }
    public GameObjectType GameObjectType { get; set; }
    public string Name { get; set; } = null!;
}