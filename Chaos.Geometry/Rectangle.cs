using System.Collections;
using Chaos.Geometry.Interfaces;

namespace Chaos.Geometry;

public class Rectangle : IRectangle, IEquatable<IRectangle>
{
    public int Bottom { get; init; }
    public int Height { get; init; }
    public int Left { get; init; }
    public int Right { get; init; }
    public int Top { get; init; }
    public int Width { get; init; }
    private IReadOnlyList<IPoint>? _vertices;

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
        Right = left + width;
        Bottom = top + height;
        _vertices = GenerateVertices();
    }

    public Rectangle(IPoint center, int width, int height)
        : this(
            center.X - (width - 1) / 2,
            center.Y - (height - 1) / 2,
            width,
            height) { }

    public IEnumerator<IPoint> GetEnumerator() => Vertices.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IReadOnlyList<IPoint> Vertices
    {
        get => _vertices ??= GenerateVertices();
        init => _vertices = value;
    }

    private IReadOnlyList<IPoint> GenerateVertices() => new List<IPoint>
    {
        new Point(Left, Top),
        new Point(Right, Top),
        new Point(Right, Bottom),
        new Point(Left, Bottom)
    };

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

        if (obj.GetType() != this.GetType())
            return false;

        return Equals((IRectangle)obj);
    }

    public override int GetHashCode() =>
        HashCode.Combine(
            Height,
            Left,
            Top,
            Width);
}