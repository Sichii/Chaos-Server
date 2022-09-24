namespace Chaos.Extensions.Common;

public static class EnumerableExtensions
{
    /// <summary>
    ///     Determines if a sequence of strings contains a specific strings in a case insensitive manner.
    /// </summary>
    public static bool ContainsI(this IEnumerable<string> enumerable, string str) =>
        enumerable.Contains(str, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Randomizes the order of the elements in a sequence
    /// </summary>
    public static List<T> Shuffle<T>(this IEnumerable<T> objects)
    {
        var list = objects.ToList();
        list.Shuffle();

        return list;
    }
}