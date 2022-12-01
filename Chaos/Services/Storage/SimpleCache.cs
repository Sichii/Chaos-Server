using Chaos.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Services.Storage;

public sealed class SimpleCache : ISimpleCache, ISimpleCacheProvider
{
    private readonly IServiceProvider Provider;
    public SimpleCache(IServiceProvider provider) => Provider = provider;

    public TResult Get<TResult>(string key) => Provider.GetRequiredService<ISimpleCache<TResult>>().Get(key);
    public ISimpleCache<T> GetCache<T>() => Provider.GetRequiredService<ISimpleCache<T>>();
}