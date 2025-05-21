#region
using Chaos.Geometry.Abstractions;
using Chaos.Pathfinding.Abstractions;
#endregion

namespace Chaos.Pathfinding;

/// <inheritdoc />
public sealed class GridDetails : IGridDetails
{
    /// <inheritdoc />
    public ICollection<IPoint> BlockingReactors { get; init; } = [];

    /// <inheritdoc />
    public int Height { get; init; }

    /// <inheritdoc />
    public ICollection<IPoint> Walls { get; init; } = [];

    /// <inheritdoc />
    public int Width { get; init; }
}