using Chaos.Geometry.Abstractions;
using Chaos.Pathfinding.Abstractions;

namespace Chaos.Pathfinding;

/// <inheritdoc />
public sealed class GridDetails : IGridDetails
{
    /// <inheritdoc />
    public ICollection<IPoint> Blacklist { get; init; } = Array.Empty<IPoint>();
    /// <inheritdoc />
    public int Height { get; init; }
    /// <inheritdoc />
    public ICollection<IPoint> Walls { get; init; } = Array.Empty<IPoint>();
    /// <inheritdoc />
    public int Width { get; init; }
}