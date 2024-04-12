using Chaos.Geometry.Abstractions;

namespace Chaos.Pathfinding.Abstractions;

/// <summary>
///     Marker interface for path options
/// </summary>
public interface IPathOptions
{
    /// <summary>
    ///     A collection of points that are blocked.
    /// </summary>
    public IReadOnlyCollection<IPoint> BlockedPoints { get; set; }

    /// <summary>
    ///     Whether to ignore blocking reactors.
    /// </summary>
    public bool IgnoreBlockingReactors { get; set; }

    /// <summary>
    ///     Whether to ignore walls.
    /// </summary>
    public bool IgnoreWalls { get; set; }

    /// <summary>
    ///     The radius to limit the pathfinding to. Defaults is 12, change only if you know what you are doing.
    /// </summary>
    public int? LimitRadius { get; set; }
}