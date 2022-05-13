using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chaos.Caches.Interfaces;
using Chaos.Options;
using Chaos.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Caches;

public class ItemTemplateCache : ISimpleCache<string, ItemTemplate>
{
    private readonly ConcurrentDictionary<string, ItemTemplate> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly ItemTemplateManagerOptions Options;

    public ItemTemplateCache(
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<ItemTemplateManagerOptions> options,
        ILogger<ItemTemplateCache> logger
    )
    {
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, ItemTemplate>(StringComparer.OrdinalIgnoreCase);
        JsonSerializerOptions = jsonSerializerOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public IEnumerator<ItemTemplate> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ItemTemplate GetObject(string key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Template key {key} was not found");

    public async Task LoadCacheAsync()
    {
        var templates = Directory.GetFiles(Options.Directory, "*.json");

        await Parallel.ForEachAsync(
            templates,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (path, token) =>
            {
                var itemTemplate = await LoadTemplateFromFileAsync(path);
                Cache.TryAdd(itemTemplate.TemplateKey, itemTemplate);
                Logger.LogTrace("Loaded item template {ItemName}", itemTemplate.TemplateKey);
            });

        Logger.LogInformation("{Count} item templates loaded", Cache.Count);
    }

    private async ValueTask<ItemTemplate> LoadTemplateFromFileAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var itemTemplate = await JsonSerializer.DeserializeAsync<ItemTemplate>(stream, JsonSerializerOptions);

        return itemTemplate!;
    }
}