using System.Threading.Tasks;

namespace Chaos.Caches.Interfaces;

public interface ISimpleCache<out TResult> : IEnumerable<TResult>
{
    TResult GetObject(string key);
    Task LoadCacheAsync();
}