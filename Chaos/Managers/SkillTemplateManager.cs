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

public class SkillTemplateManager : ICacheManager<string, SkillTemplate>
{
    private readonly ConcurrentDictionary<string, SkillTemplate> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly SkillTemplateManagerOptions Options;

    public SkillTemplateManager(IOptionsSnapshot<SkillTemplateManagerOptions> options, ILogger<SkillTemplateManager> logger)
    {
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, SkillTemplate>(StringComparer.OrdinalIgnoreCase);

        JsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        JsonSerializerOptions.Converters.Add(new PointConverter());

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public SkillTemplate GetObject(string key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Template key {key} was not found");

    public async Task LoadCacheAsync()
    {
        var templates = Directory.GetFiles(Options.Directory, "*.json");

        await Parallel.ForEachAsync(templates,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (path, token) =>
            {
                var skillTemplate = await LoadTemplateFromFileAsync(path, token);
                Cache.TryAdd(skillTemplate.TemplateKey, skillTemplate);
                Logger.LogDebug("Loaded skill template {SkillName}", skillTemplate.TemplateKey);
            });

        Logger.LogInformation("{Count} skill templates loaded", Cache.Count);
    }

    private async ValueTask<SkillTemplate> LoadTemplateFromFileAsync(string path, CancellationToken token)
    {
        await using var stream = File.OpenRead(path);
        var skillTemplate = await JsonSerializer.DeserializeAsync<SkillTemplate>(stream, JsonSerializerOptions);

        return skillTemplate!;
    }
    
    public IEnumerator<SkillTemplate> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}