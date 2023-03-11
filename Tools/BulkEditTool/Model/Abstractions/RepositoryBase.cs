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
    protected JsonSerializerOptions JsonSerializerOptions { get; }
    protected TOptions Options { get; }
    protected SynchronizedHashSet<string> Paths { get; }

    protected RepositoryBase(IOptions<TOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
    {
        Options = options.Value;
        Paths = new SynchronizedHashSet<string>(comparer: StringComparer.OrdinalIgnoreCase);
        JsonSerializerOptions = jsonSerializerOptions.Value;
        Objects = new SynchronizedList<TraceWrapper<T>>();
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => Objects.Select(wrapped => wrapped.Obj).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected virtual IEnumerable<string> GetPaths()
    {
        if (Options is not IExpiringFileCacheOptions options)
            throw new NotImplementedException("If using a different options type, override this method");

        var searchPattern = options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        return options.SearchType switch
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
        };
    }

    internal virtual async Task LoadAsync()
    {
        //load paths only once
        if (!Paths.Any())
            Paths.UnionWith(GetPaths());

        //allow reloading of objects
        if (Objects.Any())
            Objects.Clear();

        foreach (var path in Paths)
            try
            {
                var obj = await LoadFromFileAsync(path);

                if (obj == null)
                    continue;

                var wrapped = new TraceWrapper<T>(path, obj);
                Objects.Add(wrapped);
            } catch
            {
                //ignored
            }
    }

    protected virtual Task<T?> LoadFromFileAsync(string path) => JsonSerializerEx.DeserializeAsync<T>(path, JsonSerializerOptions);

    internal virtual async Task SaveChangesAsync()
    {
        foreach (var obj in Objects)
            await JsonSerializerEx.SerializeAsync(obj.Path, obj.Obj, JsonSerializerOptions);
    }
}