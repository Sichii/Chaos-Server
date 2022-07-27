using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Chaos.Core.Utilities;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Caches.Options;
using Chaos.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Caches;

public class SkillTemplateCache : ISimpleCache<SkillTemplate>
{
    private readonly ConcurrentDictionary<string, SkillTemplate> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly SkillTemplateCacheOptions Options;
    private readonly int Loaded;

    public SkillTemplateCache(
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<SkillTemplateCacheOptions> options,
        ILogger<SkillTemplateCache> logger
    )
    {
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, SkillTemplate>(StringComparer.OrdinalIgnoreCase);
        JsonSerializerOptions = jsonSerializerOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
        
        if (Interlocked.CompareExchange(ref Loaded, 1, 0) == 0)
            AsyncHelpers.RunSync(LoadCacheAsync);
    }

    public IEnumerator<SkillTemplate> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public SkillTemplate GetObject(string key) => Cache.TryGetValue(key, out var template)
        ? template
        : throw new KeyNotFoundException($"Template key {key} was not found");

    private async Task LoadCacheAsync()
    {
        var templates = Directory.GetFiles(Options.Directory, "*.json");

        await Parallel.ForEachAsync(
            templates,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (path, token) =>
            {
                var skillTemplate = await LoadTemplateFromFileAsync(path);
                Cache.TryAdd(skillTemplate.TemplateKey, skillTemplate);
                Logger.LogTrace("Loaded skill template {SkillName}", skillTemplate.TemplateKey);
            });

        Logger.LogInformation("{Count} skill templates loaded", Cache.Count);
    }

    private async Task<SkillTemplate> LoadTemplateFromFileAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var skillTemplate = await JsonSerializer.DeserializeAsync<SkillTemplate>(stream, JsonSerializerOptions);

        return skillTemplate!;
    }
}