using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Pathfinding.Abstractions;

/// <summary>
///     Defines a pattern to perform generic pathfinding
/// </summary>
public interface IPathfinder
{
    /// <summary>
    ///     Finds a path from the start to the end with options to ignorewalls or path around certain creatures
    /// </summary>
    /// <param name="start">The point to start pathfinding from</param>
    /// <param name="end">The point to pathfind to</param>
    /// <param name="ignoreWalls">Whether or not to ignore walls</param>
    /// <param name="unwalkablePoints">A collection of points to avoid</param>
    /// <returns>The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> to walk to move to the next point in the path</returns>
    Direction Pathfind(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        ICollection<IPoint> unwalkablePoints
    );

    /// <summary>
    ///     Finds a valid direction to wander
    /// </summary>
    /// <param name="start">The current point</param>
    /// <param name="ignoreWalls">Whether or not to ignore walls</param>
    /// <param name="unwalkablePoints">A collection of points to avoid</param>
    /// <returns>The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> to walk</returns>
    Direction Wander(IPoint start, bool ignoreWalls, ICollection<IPoint> unwalkablePoints);
}