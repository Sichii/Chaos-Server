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

    /// <summary>
    ///     Checks if a sequence is null or empty.
    /// </summary>
    /// <param name="enumerable">The enumerable to check</param>
    /// <typeparam name="T">The generic type of the enumerable</typeparam>
    /// <returns><c>true</c> if the sequence is null or empty, otherwise <c>false</c></returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable) => (enumerable == null) || !enumerable.Any();

    /// <summary>
    ///     Attempts to cast the IEnumerable to use a different generic type, otherwise checks each element for the given type
    /// </summary>
    /// <param name="enumerable">The enumerable to iterate</param>
    /// <typeparam name="TReturn"></typeparam>
    public static IEnumerable<TReturn> SafeCast<TReturn>(this IEnumerable enumerable)
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

    /// <summary>
    ///     Casts the given IEnumerable and then converts it to a List
    /// </summary>
    /// <param name="enumerable">The enumerable to cast and convert</param>
    /// <typeparam name="TReturn">The type to cast the IEnumerable to</typeparam>
    public static List<TReturn> ToListCast<TReturn>(this IEnumerable enumerable)
    {
        if (enumerable is List<TReturn> list)
            return list;

        return enumerable.Cast<TReturn>().ToList();
    }
}