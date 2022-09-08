using Chaos.Geometry;

namespace Chaos.Pathfinding.Abstractions;

public interface IGridDetails
{
    int Height { get; }
    ICollection<Point> Walls { get; }
    int Width { get; }
}