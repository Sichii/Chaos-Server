using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage.Abstractions;

/// <summary>
///     An <see cref="Chaos.Storage.Abstractions.ISimpleCache{TResult}"/> that loads data from a file and caches it
/// </summary>
/// <typeparam name="T">The type of object stored in the cache</typeparam>
/// <typeparam name="TSchema">The type of object the files is initially deserialized into</typeparam>
/// <typeparam name="TOptions">The type of the options object used to configure this cache</typeparam>
public abstract class SimpleFileCacheBase<T, TSchema, TOptions> : ISimpleCache<T> where TSchema: class
                                                                                  where TOptions: class, ISimpleFileCacheOptions
{
    /// <summary>
    ///     Stores cached objects
    /// </summary>
    protected ConcurrentDictionary<string, T> Cache { get; }
    
    /// <summary>
    ///     The options used for deserialization
    /// </summary>
    protected JsonSerializerOptions JsonSerializerOptions { get; }
    
    /// <summary>
    ///     A function that selects and returns the object's key
    /// </summary>
    protected abstract Func<T, string> KeySelector { get; }
    
    /// <summary>
    ///     An object used to write logs
    /// </summary>
    protected ILogger Logger { get; }
    
    /// <summary>
    ///     An object used to map the deserialized type to the return type, and any other required type conversions
    /// </summary>
    protected ITypeMapper Mapper { get; }
    
    /// <summary>
    ///     The options object used to configure this cache
    /// </summary>
    protected TOptions Options { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapper">An object used to map the deserialized type to the return type, and any other required type conversions</param>
    /// <param name="jsonSerializerOptions">The options used for deserialization</param>
    /// <param name="options">The options object used to configure this cache</param>
    /// <param name="logger">An object used to write logs</param>
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

    /// <summary>
    ///     Loads the object from the specified path and returns it
    /// </summary>
    /// <param name="path">The path to the file to deserialize</param>
    protected virtual async Task<T?> LoadFromFileAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        var schema = await JsonSerializer.DeserializeAsync<TSchema>(stream, JsonSerializerOptions);

        if (schema == null)
            return default;

        return Mapper.Map<T>(schema);
    }

    /// <inheritdoc />
    public virtual async Task ReloadAsync()
    {
        var searchPattern = Options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var sources = Options.SearchType switch
        {
            SearchType.Files       => Directory.EnumerateFiles(Options.Directory, Options.FilePattern ?? string.Empty, searchPattern),
            SearchType.Directories => Directory.EnumerateDirectories(Options.Directory, Options.FilePattern ?? string.Empty, searchPattern)
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

                // ReSharper disable once CompareNonConstrainedGenericWithNull
                if (obj == null)
                    return;

                var key = KeySelector(obj);
                Cache[key] = obj;
                Logger.LogTrace("Loaded {ObjName} \"{Key}\"", objName, key);
            });

        Logger.LogInformation("{Count} {ObjName}s loaded", Cache.Count, objName);
    }
}