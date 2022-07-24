using System.Collections;
using Chaos.Geometry.Interfaces;

namespace Chaos.Geometry;

public class Polygon : IPolygon, IEquatable<IPolygon>
{
    public IReadOnlyList<IPoint> Vertices { get; init; }

    public Polygon(IEnumerable<IPoint> vertices) => Vertices = vertices.ToList();

    public Polygon() => Vertices = new List<IPoint>();

    public IEnumerator<IPoint> GetEnumerator() => Vertices.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(IPolygon? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Vertices.Equals(other.Vertices);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;
        
        return Equals((IPolygon)obj);
    }

    public override int GetHashCode() => Vertices.GetHashCode();
}