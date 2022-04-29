using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Chaos.Core.JsonConverters;
using Chaos.Managers.Interfaces;
using Chaos.Options;
using Chaos.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Managers;

public class ItemTemplateManager : ICacheManager<string, ItemTemplate>
{
    private readonly ConcurrentDictionary<string, ItemTemplate> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly ItemTemplateManagerOptions Options;

    public ItemTemplateManager(IOptionsSnapshot<ItemTemplateManagerOptions> options, ILogger<ItemTemplateManager> logger)
    {
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, ItemTemplate>(StringComparer.OrdinalIgnoreCase);

        JsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        JsonSerializerOptions.Converters.Add(new PointConverter());

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public ItemTemplate GetObject(string key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Template key {key} was not found");

    public async Task LoadCacheAsync()
    {
        var templates = Directory.GetFiles(Options.Directory, "*.json");

        await Parallel.ForEachAsync(templates,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (path, token) =>
            {
                var itemTemplate = await LoadTemplateFromFileAsync(path, token);
                Cache.TryAdd(itemTemplate.TemplateKey, itemTemplate);
                Logger.LogDebug("Loaded item template {ItemName}", itemTemplate.TemplateKey);
            });

        Logger.LogInformation("{Count} item templates loaded", Cache.Count);
    }

    private async ValueTask<ItemTemplate> LoadTemplateFromFileAsync(string path, CancellationToken token)
    {
        await using var stream = File.OpenRead(path);
        var itemTemplate = await JsonSerializer.DeserializeAsync<ItemTemplate>(stream, JsonSerializerOptions);

        return itemTemplate!;
    }
    
    public IEnumerator<ItemTemplate> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}