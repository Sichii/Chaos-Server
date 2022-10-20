namespace Chaos.Storage.Abstractions;

public interface ISimpleCache<out TResult> : IEnumerable<TResult>
{
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