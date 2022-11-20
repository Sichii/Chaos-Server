using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage.Abstractions;

public abstract class SimpleFileCacheBase<T, TSchema, TOptions> : ISimpleCache<T> where TSchema: class
                                                                                  where TOptions: class, ISimpleFileCacheOptions
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
    public virtual T Get(string key) =>
        Cache.TryGetValue(key, out var value) ? value : throw new KeyNotFoundException($"{typeof(T).Name} Key {key} was not found");

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => Cache.Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
        var searchPattern = Options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var sources = Options.SearchType switch
        {
            SearchType.Files       => Directory.GetFiles(Options.Directory, Options.FilePattern ?? string.Empty, searchPattern),
            SearchType.Directories => Directory.GetDirectories(Options.Directory, Options.FilePattern ?? string.Empty, searchPattern)
                                               .Where(src => Directory.EnumerateFiles(src).Any()),
            _                      => throw new ArgumentOutOfRangeException()
        };

        var objName = typeof(T).Name;

        await Parallel.ForEachAsync(
            sources,
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