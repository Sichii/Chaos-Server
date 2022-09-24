using Chaos.Geometry.Abstractions;
using Chaos.Pathfinding.Abstractions;

namespace Chaos.Pathfinding;

public class GridDetails : IGridDetails
{
    public int Height { get; init; }
    public ICollection<IPoint> Walls { get; init; } = Array.Empty<IPoint>();
    public int Width { get; init; }
}