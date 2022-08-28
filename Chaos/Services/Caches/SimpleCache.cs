using Chaos.Services.Caches.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Services.Caches;

public class SimpleCache : ISimpleCache, ISimpleCacheProvider
{
    private readonly IServiceProvider Provider;
    public SimpleCache(IServiceProvider provider) => Provider = provider;
    public ISimpleCache<T> GetCache<T>() => Provider.GetRequiredService<ISimpleCache<T>>();

    public TResult GetObject<TResult>(string key) => Provider.GetRequiredService<ISimpleCache<TResult>>().GetObject(key);
}