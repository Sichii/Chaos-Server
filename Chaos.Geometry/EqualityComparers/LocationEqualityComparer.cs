using Chaos.Geometry.Abstractions;

namespace Chaos.Geometry.EqualityComparers;

public class LocationEqualityComparer : IEqualityComparer<ILocation>
{
    public static IEqualityComparer<ILocation> Instance { get; } = new LocationEqualityComparer();

    public bool Equals(ILocation? x, ILocation? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (ReferenceEquals(x, null))
            return false;

        if (ReferenceEquals(y, null))
            return false;

        return string.Equals(x.Map, y.Map, StringComparison.OrdinalIgnoreCase) && PointEqualityComparer.Instance.Equals(x, y);
    }

    public int GetHashCode(ILocation obj)
    {
        var hashCode = new HashCode();

        hashCode.Add(obj.Map, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(obj, PointEqualityComparer.Instance);

        return hashCode.ToHashCode();
    }
}