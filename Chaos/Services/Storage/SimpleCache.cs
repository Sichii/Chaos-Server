#region
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Services.Storage;

public sealed class SimpleCache(IServiceProvider provider) : ISimpleCache, ISimpleCacheProvider
{
    private readonly IServiceProvider Provider = provider;

    /// <inheritdoc />
    public bool Exists<T>(string key)
        => Provider.GetRequiredService<ISimpleCache<T>>()
                   .Exists(key);

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

    public bool TryGetValue<T>(string key, [MaybeNullWhen(false)] out T value)
        => Provider.GetRequiredService<ISimpleCache<T>>()
                   .TryGetValue(key, out value);
}