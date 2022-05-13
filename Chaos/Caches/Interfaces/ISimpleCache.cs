using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chaos.Caches.Interfaces;

public interface ISimpleCache<in TKey, out TResult> : IEnumerable<TResult>
{
    TResult GetObject(TKey key);
    Task LoadCacheAsync();
}