using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chaos.Caches.Interfaces;
using Chaos.Options;
using Chaos.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Caches;

public class SpellTemplateCache : ISimpleCache<SpellTemplate>
{
    private readonly ConcurrentDictionary<string, SpellTemplate> Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly SpellTemplateCacheOptions Options;

    public SpellTemplateCache(
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<SpellTemplateCacheOptions> options,
        ILogger<SpellTemplateCache> logger
    )
    {
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, SpellTemplate>(StringComparer.OrdinalIgnoreCase);
        JsonSerializerOptions = jsonSerializerOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public IEnumerator<SpellTemplate> GetEnumerator()
    {
        foreach (var kvp in Cache)
            yield return kvp.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public SpellTemplate GetObject(string key) => Cache.TryGetValue(key, out var template)
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
                var spellTemplate = await LoadTemplateFromFileAsync(path);
                Cache.TryAdd(spellTemplate.TemplateKey, spellTemplate);
                Logger.LogTrace("Loaded spell template {SpellName}", spellTemplate.TemplateKey);
            });

        Logger.LogInformation("{Count} spell templates loaded", Cache.Count);
    }

    private async Task<SpellTemplate> LoadTemplateFromFileAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var spellTemplate = await JsonSerializer.DeserializeAsync<SpellTemplate>(stream, JsonSerializerOptions);

        return spellTemplate!;
    }
}