using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Pathfinding.Abstractions;

/// <summary>
///     Defines a pattern for a service that performs pathfinding on any number of grids
/// </summary>
public interface IPathfindingService
{
    /// <summary>
    ///     Finds a path from the start to the end with options to ignorewalls or path around certain creatures
    /// </summary>
    /// <param name="key">The key of the grid to perform pathfinding on</param>
    /// <param name="start">The point to start pathfinding from</param>
    /// <param name="end">The point to pathfind to</param>
    /// <param name="ignoreWalls">Whether or not to ignore walls</param>
    /// <param name="unwalkablePoints">A collection of points to avoid</param>
    /// <returns>The <see cref="Direction"/> to walk to move to the next point in the path</returns>
    /// <returns></returns>
    Direction Pathfind(
        string key,
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        ICollection<IPoint> unwalkablePoints
    );

    /// <summary>
    ///     Registers a grid with the pathfinding service
    /// </summary>
    /// <param name="key">The key used to locate the grid</param>
    /// <param name="gridDetails">Required details about the grid</param>
    void RegisterGrid(
        string key,
        IGridDetails gridDetails
    );

    /// <summary>
    ///     Finds a valid direction to wander
    /// </summary>
    /// <param name="key">The key of the grid to perform pathfinding on</param>
    /// <param name="start">The current point</param>
    /// <param name="ignoreWalls">Whether or not to ignore walls</param>
    /// <param name="unwalkablePoints">A collection of points to avoid</param>
    /// <returns>The <see cref="Direction"/> to walk</returns>
    Direction Wander(
        string key,
        IPoint start,
        bool ignoreWalls,
        ICollection<IPoint> unwalkablePoints
    );
}