namespace Chaos.Common.Comparers;

/// <inheritdoc />
public sealed class ReferenceEqualityComparer : EqualityComparer<object>
{
    /// <inheritdoc />
    public override bool Equals(object? x, object? y) => ReferenceEquals(x, y);

    /// <inheritdoc />
    public override int GetHashCode(object? obj)
    {
        if (obj == null)
            return 0;

        return obj.GetHashCode();
    }
}