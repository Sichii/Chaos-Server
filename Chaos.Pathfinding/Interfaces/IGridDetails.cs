using Chaos.Geometry;

namespace Chaos.Pathfinding.Interfaces;

public interface IGridDetails
{
    int Height { get; }
    ICollection<Point> Walls { get; }
    int Width { get; }
}