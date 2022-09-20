using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chaos.Services.Mappers.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Caches.Abstractions;

public abstract class SimpleFileCacheBase<T, TSchema, TOptions> : ISimpleCache<T> where TSchema: class
                                                                                  where TOptions: class, IFileCacheOptions
{
    protected ConcurrentDictionary<string, T> Cache { get; }
    protected JsonSerializerOptions JsonSerializerOptions { get; }
    protected abstract Func<T, string> KeySelector { get; }
    protected ILogger Logger { get; }
    protected ITypeMapper Mapper { get; }
    protected TOptions Options { get; }

    protected SimpleFileCacheBase(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<TOptions> options,
        ILogger logger
    )
    {
        Mapper = mapper;
        JsonSerializerOptions = jsonSerializerOptions.Value;
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, T>(StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => Cache.Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public virtual T GetObject(string key) =>
        Cache.TryGetValue(key, out var value) ? value : throw new KeyNotFoundException($"Key {key} was not found");

    protected virtual async Task<T?> LoadFromFileAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var schema = await JsonSerializer.DeserializeAsync<TSchema>(stream, JsonSerializerOptions);

        if (schema == null)
            return default;

        return Mapper.Map<T>(schema);
    }

    public virtual async Task ReloadAsync()
    {
        var files = Options.SearchResultType switch
        {
            SearchResultType.Files       => Directory.GetFiles(Options.Directory, Options.FilePattern ?? string.Empty),
            SearchResultType.Directories => Directory.GetDirectories(Options.Directory, Options.FilePattern ?? string.Empty),
            _                            => throw new ArgumentOutOfRangeException()
        };

        var objName = typeof(T).Name;

        await Parallel.ForEachAsync(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (path, _) =>
            {
                var obj = await LoadFromFileAsync(path);

                if (obj == null)
                    return;

                var key = KeySelector(obj);
                Cache[key] = obj;
                Logger.LogTrace("Loaded {ObjName} \"{Key}\"", objName, key);
            });

        Logger.LogInformation("{Count} {ObjName}s loaded", Cache.Count, objName);
    }
}