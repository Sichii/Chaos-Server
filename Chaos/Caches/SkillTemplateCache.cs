using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chaos.Caches.Interfaces;
using Chaos.Options;
using Chaos.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Caches;

public class SkillTemplateCache : ISimpleCache<string, SkillTemplate>
{
    private readonly ConcurrentDictionary<string, SkillTemplate> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly SkillTemplateManagerOptions Options;

    public SkillTemplateCache(
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<SkillTemplateManagerOptions> options,
        ILogger<SkillTemplateCache> logger
    )
    {
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, SkillTemplate>(StringComparer.OrdinalIgnoreCase);
        JsonSerializerOptions = jsonSerializerOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
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

    public async Task LoadCacheAsync()
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

    private async ValueTask<SkillTemplate> LoadTemplateFromFileAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var skillTemplate = await JsonSerializer.DeserializeAsync<SkillTemplate>(stream, JsonSerializerOptions);

        return skillTemplate!;
    }
}