using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Text.Json;
using Chaos.Common.Collections.Synchronized;
using Chaos.Extensions.Common;
using Chaos.Storage.Abstractions.Definitions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage.Abstractions;

public abstract class ExpiringFileCacheBase<T, TSchema, TOptions> : ISimpleCache<T> where TSchema: class
                                                                                    where TOptions: class, IExpiringFileCacheOptions
{
    protected SynchronizedHashSet<string> Paths { get; set; }
    protected IMemoryCache Cache { get; }
    protected JsonSerializerOptions JsonSerializerOptions { get; }
    protected string KeyPrefix { get; }
    protected ConcurrentDictionary<string, T> LocalLookup { get; }
    protected ILogger Logger { get; }
    protected ITypeMapper Mapper { get; }
    protected TOptions Options { get; }

    protected ExpiringFileCacheBase(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<TOptions> options,
        ILogger logger
    )
    {
        Cache = cache;
        Mapper = mapper;
        Options = options.Value;
        JsonSerializerOptions = jsonSerializerOptions.Value;
        KeyPrefix = $"{typeof(T)}-";
        LocalLookup = new ConcurrentDictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        Logger = logger;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);

        Paths = LoadPaths();
    }

    /// <summary>
    ///     Searches the configured directory for an object matching the configured parameters that correlates to the provided key
    /// </summary>
    /// <param name="entry">The cache entry object</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    protected virtual T CreateFromEntry(ICacheEntry entry)
    {
        var key = entry.Key.ToString();
        var keyActual = key!.Replace(KeyPrefix, string.Empty);

        Logger.LogTrace("Creating new {TypeName} entry with key \"{Key}\"", typeof(T), key);

        entry.SetSlidingExpiration(TimeSpan.FromMinutes(Options.ExpirationMins));
        entry.RegisterPostEvictionCallback(RemoveValueCallback);

        var loadPath = Paths.FirstOrDefault(path => Path.GetFileNameWithoutExtension(path).EqualsI(keyActual));

        if (string.IsNullOrEmpty(loadPath))
            throw Options.SearchType switch
            {
                SearchType.Files => new FileNotFoundException($"Path lookup for key {keyActual} failed for type {typeof(T).Name}"),
                SearchType.Directories => new DirectoryNotFoundException(
                    $"Path lookup for key {keyActual} failed for type {typeof(T).Name}"),
                _ => new ArgumentOutOfRangeException()
            };

        var ret = LoadFromFile(loadPath);
        LocalLookup[key] = ret;

        Logger.LogTrace(
            "Created new {TypeName} entry with key \"{Key}\" from path \"{Path}\"",
            typeof(T),
            key,
            loadPath);

        return ret;
    }

    /// <inheritdoc />
    public T Get(string key) => Cache.GetOrCreate(KeyPrefix + key, CreateFromEntry);

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => LocalLookup.Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     Loads the object from the specified path and returns it
    /// </summary>
    /// <param name="path">The path to the file to deserialize</param>
    protected virtual T LoadFromFile(string path)
    {
        Logger.LogTrace("Loading new {TypeName} object from path \"{Path}\"", typeof(T).Name, path);

        using var stream = File.OpenRead(path);
        var schema = JsonSerializer.Deserialize<TSchema>(stream, JsonSerializerOptions);

        if (schema == null)
            throw new SerializationException($"Failed to serialize {typeof(TSchema).Name} from path \"{path}\"");

        return Mapper.Map<T>(schema);
    }

    protected SynchronizedHashSet<string> LoadPaths()
    {
        var searchPattern = Options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var paths = Options.SearchType switch
        {
            SearchType.Files => Directory.EnumerateFiles(Options.Directory, Options.FilePattern ?? string.Empty, searchPattern),
            SearchType.Directories => Directory.EnumerateDirectories(Options.Directory, Options.FilePattern ?? string.Empty, searchPattern)
                                               .Where(src => Directory.EnumerateFiles(src).Any()),
            _ => throw new ArgumentOutOfRangeException()
        };

        return new SynchronizedHashSet<string>(paths, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public Task ReloadAsync()
    {
        Paths = LoadPaths();

        foreach (var key in LocalLookup.Keys)
        {
            if (!Cache.TryGetValue(key, out _))
                continue;

            using var entry = Cache.CreateEntry(key);
            entry.Value = CreateFromEntry(entry);
        }

        return Task.CompletedTask;
    }

    private void RemoveValueCallback(
        object key,
        object value,
        EvictionReason reason,
        object state
    ) => LocalLookup.TryRemove(key.ToString()!, out _);
}