using System.Threading.Tasks;
using Chaos.Caches.Interfaces;
using Chaos.Data;
using Chaos.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Caches;

public class MetafileCache : ISimpleCache<Metafile>
{
    private readonly ConcurrentDictionary<string, Metafile> Cache;
    private readonly ILogger Logger;
    private readonly MetafileCacheOptions Options;

    public MetafileCache(IOptionsSnapshot<MetafileCacheOptions> options, ILogger<MetafileCache> logger)
    {
        Cache = new ConcurrentDictionary<string, Metafile>(StringComparer.OrdinalIgnoreCase);
        Options = options.Value;
        Logger = logger;
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

    public Task LoadCacheAsync() =>
        //TBD
        Task.CompletedTask;
}