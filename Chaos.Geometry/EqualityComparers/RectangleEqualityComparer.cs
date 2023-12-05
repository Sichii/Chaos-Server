using Chaos.Geometry.Abstractions;

namespace Chaos.Geometry.EqualityComparers;

/// <inheritdoc />
public sealed class RectangleEqualityComparer : IEqualityComparer<IRectangle>
{
    /// <summary>
    ///     Gets the singleton instance of this comparer.
    /// </summary>
    public static IEqualityComparer<IRectangle> Instance { get; } = new RectangleEqualityComparer();

    /// <inheritdoc />
    public bool Equals(IRectangle? x, IRectangle? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (ReferenceEquals(x, null))
            return false;

        if (ReferenceEquals(y, null))
            return false;

        if (x.GetType() != y.GetType())
            return false;

        return (x.Bottom == y.Bottom) && (x.Left == y.Left) && (x.Right == y.Right) && (x.Top == y.Top);
    }

    /// <inheritdoc />
    public int GetHashCode(IRectangle obj)
        => HashCode.Combine(
            obj.Bottom,
            obj.Left,
            obj.Right,
            obj.Top);
}