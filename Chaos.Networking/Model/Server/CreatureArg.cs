using Chaos.Core.Definitions;

namespace Chaos.Networking.Model.Server;

public record CreatureArg : VisibleArg
{
    public CreatureType CreatureType { get; set; }
    public Direction Direction { get; set; }
    public GameObjectType GameObjectType { get; set; }
    public string Name { get; set; } = null!;
}