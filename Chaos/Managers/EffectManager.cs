using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chaos.Effects.Interfaces;
using Chaos.Managers.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Managers;

public class EffectManager : ICacheManager<string, IEffect>
{
    private readonly ConcurrentDictionary<string, IEffect> Cache;
    private readonly ILogger Logger;

    public EffectManager(ILogger<EffectManager> logger)
    {
        Logger = logger;
        Cache = new ConcurrentDictionary<string, IEffect>(StringComparer.OrdinalIgnoreCase);
    }

    public IEffect GetObject(string key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Effect key {key} was not found");

    public Task LoadCacheAsync() => Task.CompletedTask;

    public IEnumerator<IEffect> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}