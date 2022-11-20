namespace Chaos.Storage.Abstractions;

/// <summary>
///     Defines a pattern for an object that caches other objects by key
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface ISimpleCache<out TResult> : IEnumerable<TResult>
{
    /// <summary>
    ///     
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    TResult Get(string key);

    Task ReloadAsync();
}

public interface ISimpleCache
{
    TResult Get<TResult>(string key);
}

public interface ISimpleCacheProvider
{
    ISimpleCache<T> GetCache<T>();
}