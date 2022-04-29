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

public class SpellTemplateManager : ICacheManager<string, SpellTemplate>
{
    private readonly ConcurrentDictionary<string, SpellTemplate> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly SpellTemplateManagerOptions Options;

    public SpellTemplateManager(IOptionsSnapshot<SpellTemplateManagerOptions> options, ILogger<SpellTemplateManager> logger)
    {
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, SpellTemplate>(StringComparer.OrdinalIgnoreCase);

        JsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        JsonSerializerOptions.Converters.Add(new PointConverter());

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public SpellTemplate GetObject(string key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Template key {key} was not found");

    public async Task LoadCacheAsync()
    {
        var templates = Directory.GetFiles(Options.Directory, "*.json");

        await Parallel.ForEachAsync(templates,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (path, token) =>
            {
                var spellTemplate = await LoadTemplateFromFileAsync(path, token);
                Cache.TryAdd(spellTemplate.TemplateKey, spellTemplate);
                Logger.LogDebug("Loaded spell template {SpellName}", spellTemplate.TemplateKey);
            });

        Logger.LogInformation("{Count} spell templates loaded", Cache.Count);
    }

    private async ValueTask<SpellTemplate> LoadTemplateFromFileAsync(string path, CancellationToken token)
    {
        await using var stream = File.OpenRead(path);
        var spellTemplate = await JsonSerializer.DeserializeAsync<SpellTemplate>(stream, JsonSerializerOptions);

        return spellTemplate!;
    }
    
    public IEnumerator<SpellTemplate> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}