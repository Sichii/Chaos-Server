using Microsoft.Extensions.Caching.Memory;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Client.Common;

public static class MemoryCacheExtensions
{
    public static T SafeGetOrCreate<T>(this IMemoryCache cache, object key, Func<ICacheEntry, T> factory)
    {
        lock (cache)
        {
            var ret = cache.GetOrCreate(key, factory);

            return ret ?? throw new NullReferenceException();
        }
    }
}