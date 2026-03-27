#region
using Chaos.Storage;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockCache<T, TSchema, TOptions> : ExpiringFileCache<T, TSchema, TOptions> where TSchema: class
    where TOptions: class, IExpiringFileCacheOptions
{
    public MockCache(
        IMemoryCache cache,
        IEntityRepository repo,
        IOptions<TOptions> options,

        // ReSharper disable once ContextualLoggerProblem
        ILogger<ExpiringFileCache<T, TSchema, TOptions>> logger)
        : base(
            cache,
            repo,
            options,
            logger) { }

    public string ConstructKeyPublic(string k) => ConstructKeyForType(k);

    public string DeconstructKeyPublic(string k) => DeconstructKeyForType(k);
    public void ForceLoadPublic() => ForceLoad();
    public string GetPathForKeyPublic(string k) => GetPathForKey(k);
    public List<string> LoadPathsPublic() => LoadPaths();

    public void RemoveValueCallbackPublic(
        object key,
        object? value,
        EvictionReason reason,
        object? state)
        => RemoveValueCallback(
            key,
            value,
            reason,
            state);
}