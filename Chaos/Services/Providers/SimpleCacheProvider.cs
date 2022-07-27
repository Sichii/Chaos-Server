using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Providers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Services.Providers;

public class SimpleCacheProvider : ISimpleCacheProvider
{
    private readonly IServiceProvider ServiceProvider;

    public SimpleCacheProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public ISimpleCache<T> GetCache<T>() => ServiceProvider.GetRequiredService<ISimpleCache<T>>();
}