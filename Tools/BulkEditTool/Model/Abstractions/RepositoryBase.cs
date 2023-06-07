using System.Collections;
using System.IO;
using System.Text.Json;
using Chaos.Common.Collections.Synchronized;
using Chaos.Common.Utilities;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Abstractions.Definitions;
using Microsoft.Extensions.Options;

namespace BulkEditTool.Model.Abstractions;

public abstract class RepositoryBase<T, TOptions> : IEnumerable<T> where T: class
                                                                   where TOptions: class
{
    public SynchronizedList<TraceWrapper<T>> Objects { get; }
    public TOptions Options { get; }
    protected JsonSerializerOptions JsonSerializerOptions { get; }
    protected SynchronizedHashSet<string> Paths { get; }

    protected RepositoryBase(IOptions<TOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
    {
        Options = options.Value;
        Paths = new SynchronizedHashSet<string>(comparer: StringComparer.OrdinalIgnoreCase);
        JsonSerializerOptions = jsonSerializerOptions.Value;
        Objects = new SynchronizedList<TraceWrapper<T>>();
    }

    public abstract void Add(string path, T obj);

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => Objects.Select(wrapped => wrapped.Object).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected virtual IEnumerable<string> GetPaths()
    {
        if (Options is not IExpiringFileCacheOptions options)
            throw new NotImplementedException("If using a different options type, override this method");

        var searchPattern = options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        return (options.SearchType switch
            {
                SearchType.Files => Directory.EnumerateFiles(
                    options.Directory,
                    options.FilePattern ?? string.Empty,
                    searchPattern),
                SearchType.Directories => Directory
                                          .EnumerateDirectories(
                                              options.Directory,
                                              options.FilePattern ?? string.Empty,
                                              searchPattern)
                                          .Where(src => Directory.EnumerateFiles(src).Any()),
                _ => throw new ArgumentOutOfRangeException()
            })
            .Select(Path.GetFullPath);
    }

    internal virtual async Task LoadAsync()
    {
        //load paths only once
        if (!Paths.Any())
            Paths.UnionWith(GetPaths());

        //allow reloading of objects
        if (Objects.Any())
            Objects.Clear();

        await Parallel.ForEachAsync(
            Paths,
            async (path, _) =>
            {
                var obj = await LoadFromFileAsync(path);

                if (obj == null)
                    return;

                var wrapped = new TraceWrapper<T>(path, obj);
                Objects.Add(wrapped);
            });
    }

    protected virtual async Task<T?> LoadFromFileAsync(string path)
    {
        try
        {
            return await JsonSerializerEx.DeserializeAsync<T>(path, JsonSerializerOptions);
        } catch (JsonException e)
        {
            throw new JsonException($"Failed to deserialize {typeof(T).Name} from path \"{path}\"", e);
        }
    }

    public abstract void Remove(string name);

    public virtual Task SaveChangesAsync() => Parallel.ForEachAsync(
        Objects,
        async (obj, _) => await SaveItemAsync(obj));

    public virtual async Task SaveItemAsync(TraceWrapper<T> wrapped)
    {
        try
        {
            var dir = Path.GetDirectoryName(wrapped.Path)!;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await JsonSerializerEx.SerializeAsync(wrapped.Path, wrapped.Object, JsonSerializerOptions);
        } catch (JsonException e)
        {
            throw new JsonException($"Failed to serialize {typeof(T).Name} to path \"{wrapped.Path}\"", e);
        }
    }
}