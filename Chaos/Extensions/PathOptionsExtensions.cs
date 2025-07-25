#region
using Chaos.DarkAges.Definitions;
using Chaos.Pathfinding;
#endregion

namespace Chaos.Extensions;

public static class PathOptionsExtensions
{
    public static PathOptions ForCreatureType(this PathOptions options, CreatureType type)
        => PathOptions.Default with
        {
            IgnoreWalls = type == CreatureType.WalkThrough
        };
}