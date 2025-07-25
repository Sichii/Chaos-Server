#region
using System.Collections;
using System.Collections.Concurrent;
using Chaos.Extensions.Common;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Abstractions.Definitions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Storage;

/// <summary>
///     An <see cref="Chaos.Storage.Abstractions.ISimpleCache{TResult}" /> that loads data from a file and caches it. The
///     data has a configurable expiration.
/// </summary>
/// <typeparam name="T">
///     The type of object stored in the cache
/// </typeparam>
/// <typeparam name="TSchema">
///     The type of object the files is initially deserialized into
/// </typeparam>
/// <typeparam name="TOptions">
/// </typeparam>
public class ExpiringFileCache<T, TSchema, TOptions> : ISimpleCache<T> where TSchema: class
                                                                       where TOptions: class, IExpiringFileCacheOptions
{
    /// <summary>
    ///     The set of paths that are used for loading data.
    /// </summary>
    protected List<string> Paths { get; set; }

    /// <summary>
    ///     The memory cache used to store the data.
    /// </summary>
    protected IMemoryCache Cache { get; }

    /// <summary>
    ///     The store used to load and save objects that map to a different type when serialized
    /// </summary>
    protected IEntityRepository EntityRepository { get; }

    /// <summary>
    ///     The prefix used for cache keys.
    /// </summary>
    protected string KeyPrefix { get; }

    /// <summary>
    ///     Many different types of objects are stored in the memory cache, this lookup is used to store the data for this
    ///     specific type
    /// </summary>
    protected ConcurrentDictionary<string, T> LocalLookup { get; }

    /// <summary>
    ///     The logger instance for logging events in this cache.
    /// </summary>
    protected virtual ILogger<ExpiringFileCache<T, TSchema, TOptions>> Logger { get; }

    /// <summary>
    ///     The options used for configuring this cache.
    /// </summary>
    protected TOptions Options { get; }

    /// <summary>
    ///     The synchronization object used to manage concurrent access to the cache.
    /// </summary>
    protected Lock Sync { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExpiringFileCache{T, TSchema, TOptions}" /> class.
    /// </summary>
    public ExpiringFileCache(
        IMemoryCache cache,
        IEntityRepository entityRepository,
        IOptions<TOptions> options,
        ILogger<ExpiringFileCache<T, TSchema, TOptions>> logger)
    {
        Options = options.Value;
        Cache = cache;
        KeyPrefix = $"{typeof(T).Name}___".ToLowerInvariant();
        LocalLookup = new ConcurrentDictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        Logger = logger;
        EntityRepository = entityRepository;
        Sync = new Lock();

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);

        Paths = LoadPaths();
    }

    /// <inheritdoc />
    public void ForceLoad()
    {
        Logger.WithTopics(Topics.Qualifiers.Forced, Topics.Actions.Load)
              .LogInformation("Force loading {@TypeName} cache", typeof(T).Name);

        using var @lock = Sync.EnterScope();

        var pathEndings = Paths.Select(Path.GetFileNameWithoutExtension);

        Parallel.ForEach(pathEndings, GetWithoutLock);

        return;

        void GetWithoutLock(string? key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            key = ConstructKeyForType(key);

            try
            {
                Cache.GetOrCreate(key, CreateFromEntry);
            } catch
            {
                Cache.Remove(key);

                throw;
            }
        }
    }

    /// <inheritdoc />
    public virtual T Get(string key)
    {
        key = ConstructKeyForType(key);

        using var @lock = Sync.EnterScope();

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
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => LocalLookup.Values.GetEnumerator();

    /// <inheritdoc />
    public virtual Task ReloadAsync()
    {
        using var @lock = Sync.EnterScope();

        Paths = LoadPaths();

        foreach (var key in LocalLookup.Keys)
            try
            {
                if (!Cache.TryGetValue(key, out _))
                {
                    Logger.WithTopics(Topics.Qualifiers.Forced, Topics.Actions.Reload)
                          .LogWarning("{@TypeName} with key {@Key} not found in cache when trying to reload it", typeof(T).Name, key);

                    continue;
                }

                using var entry = Cache.CreateEntry(key);
                entry.Value = CreateFromEntry(entry);
            } catch (Exception e)
            {
                Logger.WithTopics(Topics.Qualifiers.Forced, Topics.Actions.Reload)
                      .LogError(
                          e,
                          "Failed to reload {@TypeName} with key {@Key}",
                          typeof(T).Name,
                          key);

                //otherwise ignored
            }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task ReloadAsync(string key)
    {
        using var @lock = Sync.EnterScope();

        try
        {
            key = ConstructKeyForType(key);

            if (!Cache.TryGetValue(key, out _))
            {
                Logger.WithTopics(Topics.Qualifiers.Forced, Topics.Actions.Reload)
                      .LogWarning(
                          "{@TypeName} with key {@Key} not found in cache when trying to reload it specifically",
                          typeof(T).Name,
                          key);

                return Task.CompletedTask;
            }

            using var entry = Cache.CreateEntry(key);
            entry.Value = CreateFromEntry(entry);
        } catch (Exception e)
        {
            Logger.WithTopics(Topics.Qualifiers.Forced, Topics.Actions.Reload)
                  .LogError(
                      e,
                      "Failed to reload {@TypeName} with key {@Key}",
                      typeof(T).Name,
                      key);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Constructs a cache key for the specified object type.
    /// </summary>
    /// <param name="key">
    ///     The key to be used for the cache entry.
    /// </param>
    /// <returns>
    ///     A constructed cache key.
    /// </returns>
    protected virtual string ConstructKeyForType(string key) => KeyPrefix + key.ToLowerInvariant();

    /// <summary>
    ///     Searches the configured directory for an object matching the configured parameters that correlates to the provided
    ///     key
    /// </summary>
    /// <param name="entry">
    ///     The cache entry object
    /// </param>
    /// <returns>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// </exception>
    protected virtual T CreateFromEntry(ICacheEntry entry)
    {
        var key = entry.Key.ToString();
        var keyActual = DeconstructKeyForType(key!);

        Logger.WithTopics(Topics.Actions.Create)
              .LogDebug("Creating new {@TypeName} entry with key {@Key}", typeof(T).Name, key);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Load)
                                  .WithMetrics();

        if (Options.Expires)
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(Options.ExpirationMins!.Value));

        entry.RegisterPostEvictionCallback(RemoveValueCallback);

        var path = GetPathForKey(keyActual);

        var entity = EntityRepository.LoadAndMap<T, TSchema>(path);

        LocalLookup[key!] = entity;

        metricsLogger.LogDebug("Created new {@TypeName} entry with key {@Key}", typeof(T).Name, key);

        return entity;
    }

    /// <summary>
    ///     Deconstructs a cache key for the specified object type.
    /// </summary>
    /// <param name="key">
    ///     The key to deconstruct
    /// </param>
    protected virtual string DeconstructKeyForType(string key) => key.Replace(KeyPrefix, string.Empty, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Loads the paths for the configured directory and returns it
    /// </summary>
    /// <param name="key">
    /// </param>
    /// <returns>
    /// </returns>
    /// <exception cref="Exception">
    /// </exception>
    protected virtual string GetPathForKey(string key)
    {
        var loadPath = Paths.FirstOrDefault(path => Path.GetFileNameWithoutExtension(path)
                                                        .EqualsI(key));

        if (string.IsNullOrEmpty(loadPath))
            throw Options.SearchType switch
            {
                SearchType.Files       => new FileNotFoundException($"Path lookup for key \"{key}\" failed for type {typeof(T).Name}"),
                SearchType.Directories => new DirectoryNotFoundException($"Path lookup for key \"{key}\" failed for type {typeof(T).Name}"),
                _                      => new ArgumentOutOfRangeException()
            };

        return loadPath;
    }

    /// <summary>
    ///     Loads all potential paths from the configured directory
    /// </summary>
    /// <returns>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    protected List<string> LoadPaths()
    {
        var searchPattern = Options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var paths = Options.SearchType switch
        {
            SearchType.Files => Directory.EnumerateFiles(Options.Directory, Options.FilePattern ?? string.Empty, searchPattern),
            SearchType.Directories => Directory.EnumerateDirectories(Options.Directory, Options.FilePattern ?? string.Empty, searchPattern)
                                               .Where(src => Directory.EnumerateFiles(src)
                                                                      .Any()),
            _ => throw new ArgumentOutOfRangeException()
        };

        return paths.Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
    }

    /// <summary>
    ///     Callback for when an entry is removed from the cache
    /// </summary>
    /// <param name="key">
    /// </param>
    /// <param name="value">
    /// </param>
    /// <param name="reason">
    /// </param>
    /// <param name="state">
    /// </param>
    protected virtual void RemoveValueCallback(
        object key,
        object? value,
        EvictionReason reason,
        object? state)
    {
        //if we reload the cache, the localLookup values will automatically be replaced
        //but we dont want them to be remove by this callback
        if (reason == EvictionReason.Replaced)
            return;

        LocalLookup.TryRemove(key.ToString()!, out _);
    }
}