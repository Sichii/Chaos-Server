using System.Threading;
using System.Threading.Tasks;
using Chaos.Core.Utilities;
using Chaos.Data;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Caches.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Caches;

public class MetafileCache : ISimpleCache<Metafile>
{
    private readonly ConcurrentDictionary<string, Metafile> Cache;
    private readonly ILogger Logger;
    private readonly MetafileCacheOptions Options;
    private readonly int Loaded;

    public MetafileCache(IOptionsSnapshot<MetafileCacheOptions> options, ILogger<MetafileCache> logger)
    {
        Cache = new ConcurrentDictionary<string, Metafile>(StringComparer.OrdinalIgnoreCase);
        Options = options.Value;
        Logger = logger;
        
        if (Interlocked.CompareExchange(ref Loaded, 1, 0) == 0)
            AsyncHelpers.RunSync(LoadCacheAsync);
    }

    public IEnumerator<Metafile> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Metafile GetObject(string key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Metafile key {key} was not found");

    private Task LoadCacheAsync() =>
        //TBD
        Task.CompletedTask;
}