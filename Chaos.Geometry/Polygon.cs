using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.JsonConverters;

namespace Chaos.Geometry;

/// <inheritdoc cref="IPolygon" />
[JsonConverter(typeof(PolygonConverter))]
public sealed class Polygon : IPolygon, IEquatable<IPolygon>
{
    /// <inheritdoc />
    public IReadOnlyList<IPoint> Vertices { get; init; }

    /// <summary>
    ///     Creates a new <see cref="Polygon" /> from a sequence of vertices
    /// </summary>
    /// <param name="vertices">
    ///     A sequence of vertices. Must be contiguous, but can be either clockwise or counterclockwise. Can be convex or
    ///     concave.
    /// </param>
    public Polygon(IEnumerable<IPoint> vertices) => Vertices = vertices.ToList();

    /// <summary>
    ///     Creates a new <see cref="Polygon" /> with no vertices
    /// </summary>
    public Polygon() => Vertices = new List<IPoint>();

    /// <inheritdoc />
    public bool Equals(IPolygon? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Vertices.Equals(other.Vertices);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return Equals((IPolygon)obj);
    }

    /// <inheritdoc />
    public IEnumerator<IPoint> GetEnumerator() => Vertices.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public override int GetHashCode() => Vertices.GetHashCode();
}