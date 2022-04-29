namespace Chaos.Core.Extensions;

public static class EnumerableExtensions
{
    public static bool ContainsI(this IEnumerable<string> enumerable, string str) =>
        enumerable.Contains(str, StringComparer.OrdinalIgnoreCase);
}