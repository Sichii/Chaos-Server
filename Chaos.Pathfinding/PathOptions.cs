using Chaos.Geometry.Abstractions;
using Chaos.Pathfinding.Abstractions;

namespace Chaos.Pathfinding;

/// <summary>
///     Options for pathfinding
/// </summary>
public sealed record PathOptions : IPathOptions
{
    /// <summary>
    ///     A collection of points that are blocked.
    /// </summary>
    public IReadOnlyCollection<IPoint> BlockedPoints { get; set; } = Array.Empty<IPoint>();

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
    public int? LimitRadius { get; set; } = 12;

    /// <summary>
    ///     Default path options.
    /// </summary>
    public static PathOptions Default => new();
}