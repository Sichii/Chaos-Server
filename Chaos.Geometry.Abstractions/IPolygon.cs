namespace Chaos.Geometry.Abstractions;

/// <summary>
///     Represents a polygon with any number of sides
/// </summary>
public interface IPolygon : IEnumerable<IPoint>
{
    /// <summary>
    ///     The vertices of the polygon in clockwise order
    /// </summary>
    IReadOnlyList<IPoint> Vertices { get; }
}