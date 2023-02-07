using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Text.Json;
using Chaos.Common.Collections.Synchronized;
using Chaos.Common.Synchronization;
using Chaos.Extensions.Common;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Abstractions.Definitions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

/// <summary>
///     An <see cref="Chaos.Storage.Abstractions.ISimpleCache{TResult}" /> that loads data from a file and caches it. The data has a
///     configurable expiration.
/// </summary>
/// <typeparam name="T">The type of object stored in the cache</typeparam>
/// <typeparam name="TSchema">The type of object the files is initially deserialized into</typeparam>
/// <typeparam name="TOptions"></typeparam>
public class ExpiringFileCache<T, TSchema, TOptions> : ISimpleCache<T> where TSchema: class where TOptions: class, IExpiringFileCacheOptions
{
    protected SynchronizedHashSet<string> Paths { get; set; }
    protected IMemoryCache Cache { get; }
    protected JsonSerializerOptions JsonSerializerOptions { get; }
    protected string KeyPrefix { get; }
    protected ConcurrentDictionary<string, T> LocalLookup { get; }
    protected virtual ILogger<ExpiringFileCache<T, TSchema, TOptions>> Logger { get; }
    protected ITypeMapper Mapper { get; }
    protected TOptions Options { get; }
    protected AutoReleasingMonitor Sync { get; }

    public ExpiringFileCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<TOptions> options,
        ILogger<ExpiringFileCache<T, TSchema, TOptions>> logger
    )
    {
        Options = options.Value;
        Cache = cache;
        Mapper = mapper;
        JsonSerializerOptions = jsonSerializerOptions.Value;
        KeyPrefix = $"{typeof(T).Name}___".ToLowerInvariant();
        LocalLookup = new ConcurrentDictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        Logger = logger;
        Sync = new AutoReleasingMonitor();

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);

        Paths = LoadPaths();
    }

    protected virtual string ConstructKeyForType(string key) => KeyPrefix + key.ToLowerInvariant();

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
        var keyActual = DeconstructKeyForType(key!);

        Logger.LogTrace("Creating new {TypeName} entry with key \"{Key}\"", typeof(T), key);

        entry.SetSlidingExpiration(TimeSpan.FromMinutes(Options.ExpirationMins));
        entry.RegisterPostEvictionCallback(RemoveValueCallback);

        var path = GetPathForKey(keyActual);

        var ret = LoadFromFile(path);

        LocalLookup[key!] = ret;

        Logger.LogDebug(
            "Created new {TypeName} entry with key \"{Key}\" from path \"{Path}\"",
            typeof(T),
            key,
            path);

        return ret;
    }

    protected virtual string DeconstructKeyForType(string key) => key.Replace(KeyPrefix, string.Empty, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public void ForceLoad()
    {
        Logger.LogDebug("Force loading {TypeName} cache", typeof(T).Name);

        using var @lock = Sync.Enter();

        var pathEndings = Paths.Select(Path.GetFileNameWithoutExtension);

        foreach (var pathEnding in pathEndings)
            try
            {
                Get(pathEnding!);
            } catch
            {
                //ignored
            }
    }

    /// <inheritdoc />
    public virtual T Get(string key)
    {
        key = ConstructKeyForType(key);

        using var @lock = Sync.Enter();

        try
        {
            return Cache.GetOrCreate(key, CreateFromEntry)!;
        } catch
        {
            Cache.Remove(key);

            throw;
        }
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => LocalLookup.Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected virtual string GetPathForKey(string key)
    {
        var loadPath = Paths.FirstOrDefault(path => Path.GetFileNameWithoutExtension(path).EqualsI(key));

        if (string.IsNullOrEmpty(loadPath))
            throw Options.SearchType switch
            {
                SearchType.Files       => new FileNotFoundException($"Path lookup for key {key} failed for type {typeof(T).Name}"),
                SearchType.Directories => new DirectoryNotFoundException($"Path lookup for key {key} failed for type {typeof(T).Name}"),
                _                      => new ArgumentOutOfRangeException()
            };

        return loadPath;
    }

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

    /// <summary>
    ///     Loads all potential paths from the configured directory
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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
    public virtual Task ReloadAsync()
    {
        using var @lock = Sync.Enter();

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

    protected virtual void RemoveValueCallback(
        object key,
        object? value,
        EvictionReason reason,
        object? state
    )
    {
        //if we reload the cache, the localLookup values will automatically be replaced
        //but we dont want them to be remove by this callback
        if (reason == EvictionReason.Replaced)
            return;

        LocalLookup.TryRemove(key.ToString()!, out _);
    }
}