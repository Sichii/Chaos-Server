namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Collections.Generic.ICollection{T}" />
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    ///     Adds multiple items to a collection.
    /// </summary>
    /// <exception cref="ArgumentNullException">collection is null</exception>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(collection);

        foreach (var item in items)
            collection.Add(item);
    }

    /// <summary>
    ///     Performs a binary search on the specified collection
    /// </summary>
    /// <param name="collection">The collection the perform the search on</param>
    /// <param name="item">The item to search for</param>
    /// <param name="comparer">A comparer used to compare items</param>
    /// <typeparam name="T">The underlying type of the collection and comparer</typeparam>
    /// <returns>
    ///     The index of the item if found, otherwise the bitwise complement of the next largest item.
    ///     If no larger item is found, this will return the bitwise complement of the collection count
    /// </returns>
    public static int BinarySearch<T>(this IList<T> collection, T item, IComparer<T> comparer)
    {
        var left = 0;
        var right = collection.Count - 1;

        while (left <= right)
        {
            var mid = left + (right - left) / 2;
            var comparison = comparer.Compare(collection[mid], item);

            switch (comparison)
            {
                case 0:
                    return mid; // Item found
                case < 0:
                    left = mid + 1; // Search right part of array

                    break;
                default:
                    right = mid - 1; // Search left part of array

                    break;
            }
        }

        return
            ~left; // Item not found. Bitwise complement of the index of the next element that is larger than item, or if there is no larger element, the bitwise complement of collection.Count
    }
}