using Chaos.Services.Caches.Interfaces;

namespace Chaos.Services.Providers.Interfaces;

public interface ISimpleCacheProvider
{
    ISimpleCache<T> GetCache<T>();
}