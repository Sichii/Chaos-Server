namespace Chaos.Core.Extensions;

public static class CollectionExtensions
{
    /// <summary>
    ///     Adds multiple items to a collection.
    /// </summary>
    /// <exception cref="ArgumentNullException">collection is null</exception>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        foreach (var item in items)
            collection.Add(item);
    }
}