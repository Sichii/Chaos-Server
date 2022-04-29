using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chaos.DataObjects;
using Chaos.Managers.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Managers;

public record MetafileManagerOptions
{
    public string Directory { get; set; } = null!;
}

public class MetafileManager : ICacheManager<string, Metafile>
{
    private readonly MetafileManagerOptions Options;
    private readonly ConcurrentDictionary<string, Metafile> Cache;
    private readonly ILogger Logger;

    public MetafileManager(IOptionsSnapshot<MetafileManagerOptions> options, ILogger<MetafileManager> logger)
    {
        Cache = new ConcurrentDictionary<string, Metafile>(StringComparer.OrdinalIgnoreCase);
        Options = options.Value;
        Logger = logger;
    }

    public Metafile GetObject(string key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Metafile key {key} was not found");

    public Task LoadCacheAsync()
    {
        //TBD

        return Task.CompletedTask;
    }
    
    public IEnumerator<Metafile> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}