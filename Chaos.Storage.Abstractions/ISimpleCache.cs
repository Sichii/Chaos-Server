namespace Chaos.Storage.Abstractions;

/// <summary>
///     Defines a pattern for an object that caches other objects by key
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface ISimpleCache<out TResult> : IEnumerable<TResult>
{
    /// <summary>
    ///     Gets an object from the cache
    /// </summary>
    /// <param name="key">The key to use to find the object</param>
    TResult Get(string key);

    /// <summary>
    ///     Reloads the cache
    /// </summary>
    Task ReloadAsync();
}

/// <summary>
///     Defines a pattern for an object that fetches objects of various types from a cache
/// </summary>
public interface ISimpleCache
{
    /// <summary>
    ///     Gets an object from a cache
    /// </summary>
    /// <param name="key">The key used to find the object</param>
    /// <typeparam name="TResult">The type of object to retreive</typeparam>
    TResult Get<TResult>(string key);
}

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