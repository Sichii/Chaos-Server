using System.Collections;
using System.IO;
using System.Text.Json;
using Chaos.Collections.Synchronized;
using Chaos.Extensions.Common;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Abstractions.Definitions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Abstractions;

public abstract class RepositoryBase<T>(IEntityRepository entityRepository, IOptions<IExpiringFileCacheOptions>? options) : IEnumerable<T>
    where T: class
{
    protected IEntityRepository EntityRepository { get; } = entityRepository;
    public SynchronizedList<TraceWrapper<T>> Objects { get; } = [];
    public IExpiringFileCacheOptions? Options { get; } = options?.Value;
    protected SynchronizedHashSet<string> Paths { get; } = new(comparer: StringComparer.OrdinalIgnoreCase);

    public virtual string RootDirectory
        => Options?.Directory ?? throw new InvalidOperationException("If using a different options type, override this method");

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
        => Objects.Select(wrapped => wrapped.Object)
                  .GetEnumerator();

    public virtual TraceWrapper<T> Add(string path, T obj)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        var wrapped = new TraceWrapper<T>(path, obj);
        Objects.Add(wrapped);

        return wrapped;
    }

    protected virtual IEnumerable<string> GetPaths()
    {
        if (Options is null)
            throw new InvalidOperationException(
                "Options is null. Any class using a different options type will need to override this method");

        var searchPattern = Options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        return (Options.SearchType switch
        {
            SearchType.Files => Directory.EnumerateFiles(Options.Directory, Options.FilePattern ?? string.Empty, searchPattern),
            SearchType.Directories => Directory.EnumerateDirectories(Options.Directory, Options.FilePattern ?? string.Empty, searchPattern)
                                               .Where(
                                                   src => Directory.EnumerateFiles(src)
                                                                   .Any()),
            _ => throw new ArgumentOutOfRangeException()
        }).Select(Path.GetFullPath);
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
            return await EntityRepository.LoadAsync<T>(path);
        } catch (JsonException e)
        {
            throw new JsonException($"Failed to deserialize {typeof(T).Name} from path \"{path}\"", e);
        }
    }

    public virtual void Remove(string originalPath)
    {
        ArgumentException.ThrowIfNullOrEmpty(originalPath);

        var wrapped = Objects.FirstOrDefault(wp => wp.Path.EqualsI(originalPath));

        if (wrapped is null)
            return;

        File.Delete(wrapped.Path);
        Objects.Remove(wrapped);
    }

    public virtual Task SaveChangesAsync() => Parallel.ForEachAsync(Objects, async (obj, _) => await SaveItemAsync(obj));

    public virtual async Task SaveItemAsync(TraceWrapper<T> wrapped)
    {
        try
        {
            var dir = Path.GetDirectoryName(wrapped.Path)!;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await EntityRepository.SaveAsync(wrapped.Object, wrapped.Path);
        } catch (JsonException e)
        {
            throw new JsonException($"Failed to serialize {typeof(T).Name} to path \"{wrapped.Path}\"", e);
        }
    }
}