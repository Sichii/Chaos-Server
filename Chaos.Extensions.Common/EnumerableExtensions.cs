using System.Collections;

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Collections.Generic.IEnumerable{T}" />
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Determines if a sequence of strings contains a specific strings in a case insensitive manner.
    /// </summary>
    public static bool ContainsI(this IEnumerable<string> enumerable, string str) =>
        enumerable.Contains(str, StringComparer.OrdinalIgnoreCase);

    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable) => (enumerable == null) || !enumerable.Any();

    public static IEnumerable<TReturn> NotOfType<TReturn, TNot>(this IEnumerable<TReturn> enumerable) =>
        enumerable.Where(obj => obj is not TNot);

    public static IEnumerable<TReturn> SafeCast<TReturn, T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is IEnumerable<TReturn> casted)
            return casted;

        return enumerable.OfType<TReturn>();
    }

    /// <summary>
    ///     Randomizes the order of the elements in a sequence
    /// </summary>
    public static List<T> Shuffle<T>(this IEnumerable<T> objects)
    {
        var list = objects.ToList();
        list.ShuffleInPlace();

        return list;
    }

    public static List<TReturn> ToListCast<TReturn>(this IEnumerable enumerable)
    {
        if (enumerable is ICollection collection)
        {
            var count = collection.Count;
            var newList = new List<TReturn>(count);
            var index = 0;

            foreach (var item in collection.Cast<TReturn>())
            {
                newList[index] = item;
                index++;
            }

            return newList;
        }

        return enumerable.Cast<TReturn>().ToList();
    }
}