using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Pathfinding.Abstractions;

public interface IPathfindingService
{
    Direction Pathfind(
        string key,
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        ICollection<IPoint> unwalkablePoints
    );

    void RegisterGrid(
        string key,
        IGridDetails gridDetails
    );

    Direction Wander(
        string key,
        IPoint start,
        bool ignoreWalls,
        ICollection<IPoint> unwalkablePoints
    );
}