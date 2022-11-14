namespace Chaos.Extensions.Common;

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
}