namespace Chaos.Core.Extensions;

public static class CollectionExtensions
{
    /// <inheritdoc cref="List{T}.AddRange" />
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        foreach (var item in items)
            collection.Add(item);
    }
}