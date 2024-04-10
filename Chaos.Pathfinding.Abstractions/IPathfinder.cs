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
    /// <param name="start">
    ///     The point to start pathfinding from
    /// </param>
    /// <param name="end">
    ///     The point to pathfind to
    /// </param>
    /// <param name="ignoreWalls">
    ///     Whether or not to ignore walls
    /// </param>
    /// <param name="ignoreBlockingReactors">
    ///     Whether or not to ignore blocking reactors
    /// </param>
    /// <param name="blocked">
    ///     A collection of points to avoid
    /// </param>
    /// <param name="limitRadius">
    ///     Specify a max radius to use for path calculation, this can help with performance by limiting node discovery
    /// </param>
    Stack<IPoint> FindPath(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        bool ignoreBlockingReactors,
        IReadOnlyCollection<IPoint> blocked,
        int? limitRadius = null);

    /// <summary>
    ///     Finds a valid direction to wander
    /// </summary>
    /// <param name="start">
    ///     The current point
    /// </param>
    /// <param name="ignoreWalls">
    ///     Whether or not to ignore walls
    /// </param>
    /// <param name="ignoreBlockingReactors">
    ///     Whether or not to ignore blocking reactors
    /// </param>
    /// <param name="blocked">
    ///     A collection of points to avoid
    /// </param>
    /// <returns>
    ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> to walk
    /// </returns>
    Direction FindRandomDirection(
        IPoint start,
        bool ignoreWalls,
        bool ignoreBlockingReactors,
        IReadOnlyCollection<IPoint> blocked);

    /// <summary>
    ///     Finds a direction to walk towards the end point. No path is calculated.
    /// </summary>
    /// <param name="start">
    ///     The point to start from
    /// </param>
    /// <param name="end">
    ///     The point to walk towards
    /// </param>
    /// <param name="ignoreWalls">
    ///     Whether or not to ignore walls
    /// </param>
    /// <param name="ignoreBlockingReactors">
    ///     Whether or not to ignore blocking reactors
    /// </param>
    /// <param name="blocked">
    ///     A collection of points to avoid
    /// </param>
    /// <returns>
    ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> to move
    /// </returns>
    Direction FindSimpleDirection(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        bool ignoreBlockingReactors,
        IReadOnlyCollection<IPoint> blocked);
}