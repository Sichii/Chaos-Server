using Chaos.Geometry.Definitions;
using Chaos.Geometry.Interfaces;

namespace Chaos.Pathfinding.Interfaces;

public interface IPathfindingService
{
    Direction Pathfind(
        string key,
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        IEnumerable<IPoint> creatures
    );

    void RegisterGrid(
        string key,
        IGridDetails gridDetails
    );
}