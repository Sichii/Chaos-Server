namespace Chaos.Core.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    ///     Determines if a sequence of strings contains a specific strings in a case insensitive manner.
    /// </summary>
    public static bool ContainsI(this IEnumerable<string> enumerable, string str) =>
        enumerable.Contains(str, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Re-streams the contents of a sequence in a random order
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> objects) => objects.OrderBy(_ => Random.Shared.Next());
}