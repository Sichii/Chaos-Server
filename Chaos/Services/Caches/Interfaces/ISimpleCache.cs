namespace Chaos.Services.Caches.Interfaces;

public interface ISimpleCache<out TResult> : IEnumerable<TResult>
{
    TResult GetObject(string key);
}

public interface ISimpleCache
{
    TResult GetObject<TResult>(string key);
}

public interface ISimpleCacheProvider
{
    ISimpleCache<T> GetCache<T>();
}