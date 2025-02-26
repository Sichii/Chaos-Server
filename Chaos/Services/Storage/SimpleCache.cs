#region
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Services.Storage;

public sealed class SimpleCache(IServiceProvider provider) : ISimpleCache, ISimpleCacheProvider
{
    private readonly IServiceProvider Provider = provider;

    public T Get<T>(string key)
        => Provider.GetRequiredService<ISimpleCache<T>>()
                   .Get(key);

    public ISimpleCache<T> GetCache<T>() => Provider.GetRequiredService<ISimpleCache<T>>();

    /// <inheritdoc />
    public Task ReloadAsync<T>()
        => Provider.GetRequiredService<ISimpleCache<T>>()
                   .ReloadAsync();

    /// <inheritdoc />
    public Task ReloadAsync<T>(string key)
        => Provider.GetRequiredService<ISimpleCache<T>>()
                   .ReloadAsync(key);
}