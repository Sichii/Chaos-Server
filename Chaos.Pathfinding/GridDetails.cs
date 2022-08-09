using Chaos.Geometry;
using Chaos.Pathfinding.Interfaces;

namespace Chaos.Pathfinding;

public class GridDetails : IGridDetails
{
    public int Height { get; init; }
    public ICollection<Point> Walls { get; init; } = Array.Empty<Point>();
    public int Width { get; init; }
}