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

        var vertices = Vertices.ToList();
        var otherVertices = other.Vertices.ToList();

        if (vertices.Count != otherVertices.Count)
            return false;

        var index2 = 0;

        while (index2 < otherVertices.Count)
        {
            if (Equals(vertices[0], otherVertices[index2]))
            {
                var tempIndex1 = 0;
                var tempIndex2 = index2;
                var isSequentiallyEqual = true;

                while ((tempIndex1 < vertices.Count) && (tempIndex2 < otherVertices.Count))
                {
                    if (!Equals(vertices[tempIndex1], otherVertices[tempIndex2]))
                    {
                        isSequentiallyEqual = false;

                        break;
                    }

                    tempIndex1++;
                    tempIndex2++;

                    //allow tempIndex2 to rollover to 0 if tempIndex1 is still less than vertices.Count
                    if ((tempIndex1 < vertices.Count) && (tempIndex2 == otherVertices.Count))
                        tempIndex2 = 0;
                }

                if (isSequentiallyEqual)
                    return true;
            }

            index2++;
        }

        return false;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<IPoint> GetEnumerator() => Vertices.GetEnumerator();

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is IPolygon other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        foreach (var vertex in Vertices)
            hashCode.Add(vertex);

        return hashCode.ToHashCode();
    }
}