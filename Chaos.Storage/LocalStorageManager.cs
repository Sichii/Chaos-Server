#region
using System.Collections.Concurrent;
using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.IO.FileSystem;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Storage;

/// <summary>
///     Represents a manager that saves and loads objects in local storage
/// </summary>
public sealed class LocalStorageManager : IStorageManager
{
    private readonly IMemoryCache Cache;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly LocalStorageOptions Options;

    /// <summary>
    ///     Initializes a new instance of <see cref="LocalStorageManager" />
    /// </summary>
    public LocalStorageManager(
        IMemoryCache cache,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<LocalStorageOptions> localStorageOptions)
    {
        Cache = cache;
        JsonSerializerOptions = jsonSerializerOptions.Value;
        Options = localStorageOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    /// <inheritdoc />
    public IStorage<T> Load<T>() where T: class, new()
    {
        var entry = GetOrAddEntry<T>();

        return new StorageObject<T>(this, entry);
    }

    /// <inheritdoc />
    public async Task<IStorage<T>> LoadAsync<T>() where T: class, new()
    {
        var entry = await GetOrAddEntryAsync<T>();

        return new StorageObject<T>(this, entry);
    }

    /// <inheritdoc />
    public void Save<T>(IStorage<T> obj) where T: class, new()
    {
        var entry = GetOrAddEntry<T>();

        entry[((ReadOnlyStorageObject<T>)obj).Name] = obj.Value;

        var fileName = $"{typeof(T).Name}.json";
        var filePath = Path.Combine(Options.Directory, fileName);

        filePath.SafeExecute(path => JsonSerializerEx.Serialize(path, entry.ToDictionary(), JsonSerializerOptions));
    }

    /// <inheritdoc />
    public async Task SaveAsync<T>(IStorage<T> obj) where T: class, new()
    {
        var entry = await GetOrAddEntryAsync<T>();

        entry[((ReadOnlyStorageObject<T>)obj).Name] = obj.Value;

        var fileName = $"{typeof(T).Name}.json";
        var filePath = Path.Combine(Options.Directory, fileName);

        await filePath.SafeExecuteAsync(path => JsonSerializerEx.SerializeAsync(path, entry.ToDictionary(), JsonSerializerOptions));
    }

    #region Internals
    private string ConstructKey<T>() => $"local__{typeof(T).Name}";

    internal ConcurrentDictionary<string, T> GetOrAddEntry<T>()
    {
        var key = ConstructKey<T>();

        return Cache.GetOrCreate(key, _ => LoadOrCreateEntry<T>())!;
    }

    internal Task<ConcurrentDictionary<string, T>> GetOrAddEntryAsync<T>()
    {
        var key = ConstructKey<T>();

        return Cache.GetOrCreateAsync(key, _ => LoadOrCreateEntryAsync<T>())!;
    }

    internal ConcurrentDictionary<string, T> LoadOrCreateEntry<T>()
    {
        var fileName = $"{typeof(T).Name}.json";
        var filePath = Path.Combine(Options.Directory, fileName);

        return filePath.SafeExecute(
            path =>
            {
                if (File.Exists(path))
                {
                    var data = JsonSerializerEx.Deserialize<Dictionary<string, T>>(path, JsonSerializerOptions)!;

                    return new ConcurrentDictionary<string, T>(data, StringComparer.OrdinalIgnoreCase);
                }

                return new ConcurrentDictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            });
    }

    internal Task<ConcurrentDictionary<string, T>> LoadOrCreateEntryAsync<T>()
    {
        var fileName = $"{typeof(T).Name}.json";
        var filePath = Path.Combine(Options.Directory, fileName);

        return filePath.SafeExecuteAsync(
            async path =>
            {
                if (File.Exists(path))
                {
                    var data = await JsonSerializerEx.DeserializeAsync<Dictionary<string, T>>(path, JsonSerializerOptions);

                    return new ConcurrentDictionary<string, T>(data!, StringComparer.OrdinalIgnoreCase);
                }

                return new ConcurrentDictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            });
    }
    #endregion
}