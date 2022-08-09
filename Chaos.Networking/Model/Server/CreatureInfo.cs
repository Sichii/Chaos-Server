using Chaos.Geometry.Definitions;
using Chaos.Networking.Definitions;

namespace Chaos.Networking.Model.Server;

public record CreatureInfo : VisibleObjectInfo
{
    public CreatureType CreatureType { get; set; }
    public Direction Direction { get; set; }
    public GameObjectType GameObjectType { get; set; }
    public string Name { get; set; } = null!;
}