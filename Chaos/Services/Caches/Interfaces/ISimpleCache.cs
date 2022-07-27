namespace Chaos.Services.Caches.Interfaces;

public interface ISimpleCache<out TResult> : IEnumerable<TResult>
{
    TResult GetObject(string key);
}