#region
using System.Collections;
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Geometry;

/// <summary>
///     Represents a rectangle, a 4 sided polygon
/// </summary>
public readonly ref struct ValueRectangle : IRectangle, IEquatable<IRectangle>
{
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
    public IReadOnlyList<IPoint> Vertices { get; init; }

    /// <inheritdoc />
    public int Width { get; init; }

    /// <inheritdoc />
    public int Area => Height * Width;

    /// <summary>
    ///     Creates a new <see cref="ValueRectangle" /> from values indicating the top left corner, width, and height
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
    public ValueRectangle(
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
        Vertices = GenerateVertices();
    }

    /// <summary>
    ///     Implicitly converts a rectangle to a ref struct rectangle
    /// </summary>
    public static explicit operator ValueRectangle(Rectangle rect)
        => new(
            rect.Left,
            rect.Top,
            rect.Width,
            rect.Height);

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
    public ValueRectangle(IPoint center, int width, int height)
        : this(
            center.X - (width - 1) / 2,
            center.Y - (height - 1) / 2,
            width,
            height) { }

    /// <inheritdoc />
    public bool Equals(IRectangle? other)
        => other is not null && (Left == other.Left) && (Top == other.Top) && (Width == other.Width) && (Height == other.Height);

    /// <inheritdoc />
    public IEnumerator<IPoint> GetEnumerator() => Vertices.GetEnumerator();

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is IRectangle other && Equals(other);

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

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}