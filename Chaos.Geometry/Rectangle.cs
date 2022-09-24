using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.JsonConverters;

namespace Chaos.Geometry;

[JsonConverter(typeof(RectangleConverter))]
public class Rectangle : IRectangle, IEquatable<IRectangle>
{
    private IReadOnlyList<IPoint>? _vertices;
    /// <summary>
    ///     The highest Y coordinate of the rectangle
    /// </summary>
    public int Bottom { get; init; }
    /// <summary>
    ///     The height of the rectangle
    /// </summary>
    public int Height { get; init; }
    /// <summary>
    ///     The lowest X coordinate of the rectangle
    /// </summary>
    public int Left { get; init; }
    /// <summary>
    ///     The highest X coordinate of the rectangle
    /// </summary>
    public int Right { get; init; }
    /// <summary>
    ///     The lowest Y coordinate of the rectangle
    /// </summary>
    public int Top { get; init; }

    public IReadOnlyList<IPoint> Vertices
    {
        get => _vertices ??= GenerateVertices();
        init => _vertices = value;
    }

    /// <summary>
    ///     The width of the rectangle
    /// </summary>
    public int Width { get; init; }

    public int Area => Height * Width;

    public Rectangle() { }

    public Rectangle(
        int left,
        int top,
        int width,
        int height
    )
    {
        Width = width;
        Height = height;
        Left = left;
        Top = top;
        Right = left + width - 1;
        Bottom = top + height - 1;
        _vertices = GenerateVertices();
    }

    public Rectangle(IPoint center, int width, int height)
        : this(
            center.X - (width - 1) / 2,
            center.Y - (height - 1) / 2,
            width,
            height) { }

    public bool Equals(IRectangle? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return (Height == other.Height)
               && (Left == other.Left)
               && (Top == other.Top)
               && (Width == other.Width);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return Equals((IRectangle)obj);
    }

    private IReadOnlyList<IPoint> GenerateVertices() => new List<IPoint>
    {
        new Point(Left, Top),
        new Point(Right, Top),
        new Point(Right, Bottom),
        new Point(Left, Bottom)
    };

    public IEnumerator<IPoint> GetEnumerator() => Vertices.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override int GetHashCode() =>
        HashCode.Combine(
            Height,
            Left,
            Top,
            Width);
}