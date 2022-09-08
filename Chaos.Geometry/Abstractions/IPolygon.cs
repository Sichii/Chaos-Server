namespace Chaos.Geometry.Abstractions;

public interface IPolygon : IEnumerable<IPoint>
{
    IReadOnlyList<IPoint> Vertices { get; }
}