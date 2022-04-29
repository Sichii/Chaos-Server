using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chaos.Managers.Interfaces;

public interface ICacheManager<in TKey, out TResult> : IEnumerable<TResult>
{
    TResult GetObject(TKey key);
    Task LoadCacheAsync();
}