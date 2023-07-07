using System.Collections;
using System.Numerics;

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
    ///     Finds the next highest number in a sequence from a given value
    /// </summary>
    /// <param name="enumerable">The sequence to search</param>
    /// <param name="seed">The starting value</param>
    /// <typeparam name="T">A numeric type</typeparam>
    public static T NextHighest<T>(this IEnumerable<T> enumerable, T seed) where T: INumber<T>
    {
        var current = seed;

        foreach (var number in enumerable)
        {
            //dont consider any numbers that are less than or equal to the seed
            if (number <= seed)
                continue;

            //if the current number is the seed, take the first number that reaches this statement
            //only numbers that are greater than the seed will reach this statement
            if (current == seed)
                current = number;
            //otherwise, if the number is less than the current number, take it
            //all numbers that reach this statement are greater than the seed
            else if (number < current)
                current = number;
        }

        return current;
    }

    /// <summary>
    ///     Finds the next lowest number in a sequence from a given value
    /// </summary>
    /// <param name="enumerable">The sequence to search</param>
    /// <param name="seed">The starting value</param>
    /// <typeparam name="T">A numeric type</typeparam>
    public static T NextLowest<T>(this IEnumerable<T> enumerable, T seed) where T: INumber<T>
    {
        var current = seed;

        foreach (var number in enumerable)
        {
            //dont consider any numbers that are greater than or equal to the seed
            if (number >= seed)
                continue;

            //if the current number is the seed, take the first number that reaches this statement
            //only numbers that are less than the seed will reach this statement
            if (current == seed)
                current = number;
            //otherwise, if the number is greater than the current number, take it
            //all numbers that reach this statement are lower than the seed
            else if (number > current)
                current = number;
        }

        return current;
    }

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

        return enumerable.SafeCast<TReturn>().ToList();
    }
}