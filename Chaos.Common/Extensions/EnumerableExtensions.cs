#region
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using Chaos.Common.Utilities;
#endregion

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Collections.Generic.IEnumerable{T}" />
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Determines if a sequence of strings contains a specific strings in a case insensitive manner.
    /// </summary>
    public static bool ContainsI(this IEnumerable<string> enumerable, string str)
        => enumerable.Contains(str, StringComparer.OrdinalIgnoreCase);

    /// <param name="enumerable">
    ///     The sequence to search
    /// </param>
    /// <typeparam name="T">
    ///     A numeric type
    /// </typeparam>
    extension<T>(IEnumerable<T> enumerable) where T: INumber<T>
    {
        /// <summary>
        ///     Finds the next highest number in a sequence from a given value
        /// </summary>
        /// <param name="seed">
        ///     The starting value
        /// </param>
        public T NextHighest(T seed)
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
        /// <param name="seed">
        ///     The starting value
        /// </param>
        public T NextLowest(T seed)
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
    }

    /// <param name="enumerable">
    ///     The source of items
    /// </param>
    /// <typeparam name="T">
    ///     The type of the item
    /// </typeparam>
    extension<T>(IEnumerable<T> enumerable)
    {
        /// <summary>
        ///     Orders the given enumerable by the given comparer
        /// </summary>
        public IOrderedEnumerable<T> OrderBy(IComparer<T> comparer) => enumerable.OrderBy(e => e, comparer);

        /// <summary>
        ///     Orders the given enumerable by the given comparer
        /// </summary>
        public IOrderedEnumerable<T> OrderByDescending(IComparer<T> comparer) => enumerable.OrderByDescending(e => e, comparer);

        /// <summary>
        ///     Rents a span from the shared <see cref="ArrayPool{T}" /> and fills it with the elements of the enumerable. The
        ///     caller is responsible for disposing the returned <see cref="Rented{T}" />.
        /// </summary>
        /// <param name="count">
        ///     The expected count
        /// </param>
        public Rented<T> ToRented(int count)
        {
            var rentedSpan = new Rented<T>(count);
            var span = rentedSpan.Span;
            var index = 0;

            foreach (var item in enumerable)
                span[index++] = item;

            return rentedSpan;
        }

        /// <summary>
        ///     Rents a span from the shared <see cref="ArrayPool{T}" /> and fills it with the elements of the enumerable. If the
        ///     count is not known, a doubling buffer strategy is used. The caller is responsible for disposing the returned
        ///     <see cref="Rented{T}" />.
        /// </summary>
        public Rented<T> ToRented()
        {
            if (enumerable.TryGetNonEnumeratedCount(out var count))
                return enumerable.ToRented(count);

            var array = ArrayPool<T>.Shared.Rent(32);
            var index = 0;

            foreach (var item in enumerable)
            {
                if (index == array.Length)
                {
                    var newArray = ArrayPool<T>.Shared.Rent(array.Length * 2);

                    array.AsSpan(0, index)
                         .CopyTo(newArray);

                    ArrayPool<T>.Shared.Return(array);

                    array = newArray;
                }

                array[index++] = item;
            }

            return new Rented<T>(array, index);
        }
    }

    extension<T>(IOrderedEnumerable<T> orderedEnumerable)
    {
        /// <summary>
        ///     Orders the given enumerable by the given comparer
        /// </summary>
        public IOrderedEnumerable<T> ThenBy(IComparer<T> comparer) => orderedEnumerable.ThenBy(e => e, comparer);

        /// <summary>
        ///     Orders the given enumerable by the given comparer
        /// </summary>
        public IOrderedEnumerable<T> ThenByDescending(IComparer<T> comparer) => orderedEnumerable.ThenByDescending(e => e, comparer);
    }
}