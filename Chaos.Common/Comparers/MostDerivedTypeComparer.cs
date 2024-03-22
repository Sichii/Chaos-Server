namespace Chaos.Common.Comparers;

/// <summary>
///     Compares two types to determine which is the most derived.
/// </summary>
public sealed class MostDerivedTypeComparer : IComparer<Type>
{
    /// <summary>
    ///     Gets the singleton instance of the <see cref="MostDerivedTypeComparer" />.
    /// </summary>
    public static MostDerivedTypeComparer Instance { get; } = new();

    /// <inheritdoc />
    public int Compare(Type? x, Type? y)
    {
        if (x == y)
            return 0;

        if (x == null)
            return -1;

        if (y == null)
            return 1;

        if (x.IsAssignableTo(y))
            return -1;

        if (y.IsAssignableTo(x))
            return 1;

        return 0;
    }
}