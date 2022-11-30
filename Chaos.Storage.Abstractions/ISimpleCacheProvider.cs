namespace Chaos.Storage.Abstractions;

/// <summary>
///     Defines a pattern for an object that fetches caches for different types
/// </summary>
public interface ISimpleCacheProvider
{
    /// <summary>
    ///     Gets a cache
    /// </summary>
    /// <typeparam name="T">The type of object the cache stores</typeparam>
    ISimpleCache<T> GetCache<T>();
}