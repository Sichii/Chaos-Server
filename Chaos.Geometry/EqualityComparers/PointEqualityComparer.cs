using Chaos.Geometry.Abstractions;

namespace Chaos.Geometry.EqualityComparers;

/// <inheritdoc />
public sealed class PointEqualityComparer : IEqualityComparer<IPoint>
{
    /// <summary>
    ///     The singleton instance of this comparer
    /// </summary>
    public static IEqualityComparer<IPoint> Instance { get; } = new PointEqualityComparer();

    /// <inheritdoc />
    public bool Equals(IPoint? x, IPoint? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (ReferenceEquals(x, null))
            return false;

        if (ReferenceEquals(y, null))
            return false;

        return (x.X == y.X) && (x.Y == y.Y);
    }

    /// <inheritdoc />
    public int GetHashCode(IPoint obj) => (obj.X << 16) + obj.Y;
}