using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.JsonConverters;

namespace Chaos.Geometry;

/// <inheritdoc cref="IRectangle" />
[JsonConverter(typeof(RectangleConverter))]
public sealed class Rectangle : IRectangle, IEquatable<IRectangle>
{
    private IReadOnlyList<IPoint>? _vertices;

    /// <inheritdoc />
    public int Bottom { get; init; }

    /// <inheritdoc />
    public int Height { get; init; }

    /// <inheritdoc />
    public int Left { get; init; }

    /// <inheritdoc />
    public int Right { get; init; }

    /// <inheritdoc />
    public int Top { get; init; }

    /// <inheritdoc />
    public IReadOnlyList<IPoint> Vertices
    {
        get => _vertices ??= GenerateVertices();
        init => _vertices = value;
    }

    /// <inheritdoc />
    public int Width { get; init; }

    /// <inheritdoc />
    public int Area => Height * Width;

    /// <summary>
    ///     Creates a new <see cref="Rectangle" /> with no vertices
    /// </summary>
    public Rectangle() { }

    /// <summary>
    ///     Creates a new <see cref="Rectangle" /> from values indicating the top left corner, width, and height
    /// </summary>
    /// <param name="left">
    ///     The X value of the rectangle's leftmost edge
    /// </param>
    /// <param name="top">
    ///     The Y value of the retangle's topmost edge
    /// </param>
    /// <param name="width">
    ///     The width of the rectangle
    /// </param>
    /// <param name="height">
    ///     The height of the rectangle
    /// </param>
    public Rectangle(
        int left,
        int top,
        int width,
        int height)
    {
        Width = width;
        Height = height;
        Left = left;
        Top = top;
        Right = left + width - 1;
        Bottom = top + height - 1;
        _vertices = GenerateVertices();
    }

    /// <summary>
    ///     Creates a new <see cref="Rectangle" /> from the center point, width, and height
    /// </summary>
    /// <param name="center">
    ///     The center point of the rectangle
    /// </param>
    /// <param name="width">
    ///     The width of the rectangle. The width must be an odd number.
    /// </param>
    /// <param name="height">
    ///     The height of the rectangle. The height must be an odd number.
    /// </param>
    public Rectangle(IPoint center, int width, int height)
        : this(
            center.X - (width - 1) / 2,
            center.Y - (height - 1) / 2,
            width,
            height) { }

    /// <inheritdoc />
    public bool Equals(IRectangle? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return (Height == other.Height) && (Left == other.Left) && (Top == other.Top) && (Width == other.Width);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<IPoint> GetEnumerator() => Vertices.GetEnumerator();

    /// <inheritdoc />
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

    private IReadOnlyList<IPoint> GenerateVertices()
        => new List<IPoint>
        {
            new Point(Left, Top),
            new Point(Right, Top),
            new Point(Right, Bottom),
            new Point(Left, Bottom)
        };

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(
            Height,
            Left,
            Top,
            Width);
}