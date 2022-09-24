using Chaos.Geometry.Abstractions;

namespace Chaos.Pathfinding.Abstractions;

public interface IGridDetails
{
    int Height { get; }
    ICollection<IPoint> Walls { get; }
    int Width { get; }
}