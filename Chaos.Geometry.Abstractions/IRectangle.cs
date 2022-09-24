namespace Chaos.Geometry.Abstractions;

public interface IRectangle : IPolygon
{
    int Bottom { get; }
    int Height { get; }
    int Left { get; }
    int Right { get; }
    int Top { get; }
    int Width { get; }
}