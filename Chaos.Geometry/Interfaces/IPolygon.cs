namespace Chaos.Geometry.Interfaces;

public interface IPolygon : IEnumerable<IPoint>
{
    IReadOnlyList<IPoint> Vertices { get; }
}