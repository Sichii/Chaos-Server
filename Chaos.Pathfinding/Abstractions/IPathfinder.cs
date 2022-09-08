using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Definitions;

namespace Chaos.Pathfinding.Abstractions;

public interface IPathfinder
{
    Direction Pathfind(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        ICollection<IPoint> creatures
    );

    Direction Wander(IPoint start, bool ignoreWalls, ICollection<IPoint> creatures);
}