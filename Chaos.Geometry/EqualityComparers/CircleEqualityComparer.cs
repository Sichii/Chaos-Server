using Chaos.Geometry.Abstractions;

namespace Chaos.Geometry.EqualityComparers;

/// <inheritdoc />
public sealed class CircleEqualityComparer : IEqualityComparer<ICircle>
{
    /// <summary>
    ///     The singleton instance of this comparer
    /// </summary>
    public static IEqualityComparer<ICircle> Instance { get; } = new CircleEqualityComparer();

    /// <inheritdoc />
    public bool Equals(ICircle? x, ICircle? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (ReferenceEquals(x, null))
            return false;

        if (ReferenceEquals(y, null))
            return false;

        if (x.GetType() != y.GetType())
            return false;

        return PointEqualityComparer.Instance.Equals(x.Center, y.Center) && (x.Radius == y.Radius);
    }

    /// <inheritdoc />
    public int GetHashCode(ICircle obj) => HashCode.Combine(PointEqualityComparer.Instance.GetHashCode(obj.Center), obj.Radius);
}