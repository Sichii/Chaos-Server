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
    /// <param name="gridKey">
    ///     The key of the grid to perform pathfinding on
    /// </param>
    /// <param name="start">
    ///     The point to start pathfinding from
    /// </param>
    /// <param name="end">
    ///     The point to pathfind to
    /// </param>
    /// <param name="ignoreWalls">
    ///     Whether or not to ignore walls
    /// </param>
    /// <param name="blocked">
    ///     A collection of points to avoid
    /// </param>
    /// <param name="limitRadius">
    ///     Specify a max radius to use for path calculation, this can help with performance by limiting node discovery
    /// </param>
    /// <returns>
    ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> to walk to move to the next point in the path
    /// </returns>
    /// <returns>
    /// </returns>
    Direction Pathfind(
        string gridKey,
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        IReadOnlyCollection<IPoint> blocked,
        int? limitRadius = null);

    /// <summary>
    ///     Registers a grid with the pathfinding service
    /// </summary>
    /// <param name="key">
    ///     The key used to locate the grid
    /// </param>
    /// <param name="gridDetails">
    ///     Required details about the grid
    /// </param>
    void RegisterGrid(string key, IGridDetails gridDetails);

    /// <summary>
    ///     Finds a direction to walk towards the end point. No path is calculated.
    /// </summary>
    /// <param name="gridKey">
    ///     The key of the grid to find a path on
    /// </param>
    /// <param name="start">
    ///     The point to start from
    /// </param>
    /// <param name="end">
    ///     The point to walk towards
    /// </param>
    /// <param name="ignoreWalls">
    ///     Whether or not to ignore walls
    /// </param>
    /// <param name="blocked">
    ///     A collection of points to avoid
    /// </param>
    /// <returns>
    ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> to move
    /// </returns>
    public Direction SimpleWalk(
        string gridKey,
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        IReadOnlyCollection<IPoint> blocked);

    /// <summary>
    ///     Finds a valid direction to wander
    /// </summary>
    /// <param name="key">
    ///     The key of the grid to perform pathfinding on
    /// </param>
    /// <param name="start">
    ///     The current point
    /// </param>
    /// <param name="ignoreWalls">
    ///     Whether or not to ignore walls
    /// </param>
    /// <param name="blocked">
    ///     A collection of points to avoid
    /// </param>
    /// <returns>
    ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> to walk
    /// </returns>
    Direction Wander(
        string key,
        IPoint start,
        bool ignoreWalls,
        IReadOnlyCollection<IPoint> blocked);
}