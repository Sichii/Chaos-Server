namespace Chaos.Common.Comparers;

/// <summary>
///     Represents a comparer that uses a lambda expression to compare two objects.
/// </summary>
public sealed class LambdaComparer<T> : IComparer<T>
{
    private readonly bool Ascending;
    private readonly Func<T?, T?, int> CompareFunc;

    private LambdaComparer(Func<T?, T?, int> compareFunc, bool ascending = true)
    {
        CompareFunc = compareFunc;
        Ascending = ascending;
    }

    /// <inheritdoc />
    public int Compare(T? x, T? y) => Ascending ? CompareFunc(x, y) : CompareFunc(y, x);

    /// <summary>
    ///     Creates a new <see cref="LambdaComparer{T}" /> from the given <paramref name="compareFunc" />.
    /// </summary>
    public static LambdaComparer<T> Create(Func<T?, T?, int> compareFunc, bool ascending = true) => new(compareFunc, ascending);

    /// <summary>
    ///     Creates a new <see cref="LambdaComparer{T}" /> from the given <paramref name="selectFunc" />.
    /// </summary>
    public static LambdaComparer<T> FromSelect<TSelected>(Func<T?, TSelected?> selectFunc, bool ascending = true)
        where TSelected: IComparable
        => new(
            (x, y) =>
            {
                var selectedX = selectFunc(x);
                var selectedY = selectFunc(y);

                if (selectedX is null || selectedY is null)
                    return 0;

                return selectedX.CompareTo(selectedY);
            },
            ascending);
}