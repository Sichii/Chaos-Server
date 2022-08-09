using Chaos.Geometry.Definitions;
using Chaos.Geometry.Interfaces;

namespace Chaos.Pathfinding.Interfaces;

public interface IPathfinder
{
    Direction Pathfind(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        IEnumerable<IPoint> creatures
    );
}