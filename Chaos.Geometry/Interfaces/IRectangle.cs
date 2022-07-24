namespace Chaos.Geometry.Interfaces;

public interface IRectangle : IPolygon
{
    int Bottom { get; }
    int Height { get; }
    int Left { get; }
    int Right { get; }
    int Top { get; }
    int Width { get; }
}