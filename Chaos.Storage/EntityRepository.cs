#region
using System.Runtime.Serialization;
using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Storage;

/// <summary>
///     Represents a store that can load and save objects that map to a different type when serialized
/// </summary>
public sealed class EntityRepository : IEntityRepository
{
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger<EntityRepository> Logger;
    private readonly ITypeMapper Mapper;
    private readonly EntityRepositoryOptions Options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityRepository" /> class
    /// </summary>
    public EntityRepository(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonserializerOptions,
        IOptionsSnapshot<EntityRepositoryOptions> options,
        ILogger<EntityRepository> logger)
    {
        Mapper = mapper;
        Logger = logger;
        JsonSerializerOptions = jsonserializerOptions.Value;
        Options = options.Value;
    }

    /// <inheritdoc />
    public TSchema Load<TSchema>(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading new unmapped {@TypeName} object from path {@Path}", typeof(TSchema).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Load)
                                  .WithMetrics();

        var schema = JsonSerializerEx.Deserialize<TSchema>(path, JsonSerializerOptions);

        if (schema is null)
            throw new SerializationException($"Failed to serialize unmapped {typeof(TSchema).Name} from path \"{path}\"");

        metricsLogger.LogTrace("Loaded new unmapped {@TypeName} object from path {@Path}", typeof(TSchema).Name, path);

        return schema;
    }

    /// <inheritdoc />
    public T LoadAndMap<T, TSchema>(string path, Action<TSchema>? preMapAction = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading new {@TypeName} object from path {@Path}", typeof(T).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Load)
                                  .WithMetrics();

        var schema = JsonSerializerEx.Deserialize<TSchema>(path, JsonSerializerOptions);

        if (schema is null)
            throw new SerializationException($"Failed to serialize {typeof(TSchema).Name} from path \"{path}\"");

        preMapAction?.Invoke(schema);

        var ret = Mapper.Map<T>(schema);

        metricsLogger.LogTrace("Loaded new {@TypeName} object from path {@Path}", typeof(T).Name, path);

        return ret;
    }

    /// <inheritdoc />
    public async Task<T> LoadAndMapAsync<T, TSchema>(string path, Func<TSchema, Task>? preMapAction = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading new {@TypeName} object from path {@Path}", typeof(T).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Load)
                                  .WithMetrics();

        var schema = await JsonSerializerEx.DeserializeAsync<TSchema>(path, JsonSerializerOptions);

        if (schema is null)
            throw new SerializationException($"Failed to serialize {typeof(TSchema).Name} from path \"{path}\"");

        if (preMapAction is not null)
            await preMapAction(schema);

        var ret = Mapper.Map<T>(schema);

        metricsLogger.LogTrace("Loaded new {@TypeName} object from path {@Path}", typeof(T).Name, path);

        return ret;
    }

    /// <inheritdoc />
    public IEnumerable<T> LoadAndMapMany<T, TSchema>(string path, Action<TSchema>? preMapAction = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading collection of {@TypeName} objects from path {@Path}", typeof(T).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Load)
                                  .WithMetrics();

        var schemas = JsonSerializerEx.Deserialize<ICollection<TSchema>>(path, JsonSerializerOptions)!;

        if (schemas is null)
            throw new SerializationException($"Failed to serialize collection of {typeof(TSchema).Name} from path \"{path}\"");

        foreach (var schema in schemas)
            preMapAction?.Invoke(schema);

        foreach (var mapped in Mapper.MapMany<TSchema, T>(schemas))
            yield return mapped;

        metricsLogger.LogTrace("Loaded collection of {@TypeName} objects from path {@Path}", typeof(T).Name, path);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<T> LoadAndMapManyAsync<T, TSchema>(string path, Func<TSchema, Task>? preMapAction = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading collection of {@TypeName} objects from path {@Path}", typeof(T).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Load)
                                  .WithMetrics();

        var schemas = await JsonSerializerEx.DeserializeAsync<ICollection<TSchema>>(path, JsonSerializerOptions);

        if (schemas is null)
            throw new SerializationException($"Failed to serialize collection of {typeof(TSchema).Name} from path \"{path}\"");

        if (preMapAction is not null)
            foreach (var schema in schemas)
            {
                await preMapAction(schema);

                yield return Mapper.Map<T>(schema!);
            }
        else
            foreach (var obj in Mapper.MapMany<TSchema, T>(schemas))
                yield return obj;

        metricsLogger.LogTrace("Loaded collection of {@TypeName} objects from path {@Path}", typeof(T).Name, path);
    }

    /// <inheritdoc />
    public async Task<TSchema> LoadAsync<TSchema>(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading new unmapped {@TypeName} object from path {@Path}", typeof(TSchema).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Load)
                                  .WithMetrics();

        var schema = await JsonSerializerEx.DeserializeAsync<TSchema>(path, JsonSerializerOptions);

        if (schema is null)
            throw new SerializationException($"Failed to serialize unmapped {typeof(TSchema).Name} from path \"{path}\"");

        metricsLogger.LogTrace("Loaded new unmapped {@TypeName} object from path {@Path}", typeof(TSchema).Name, path);

        return schema;
    }

    /// <inheritdoc />
    public IEnumerable<TSchema> LoadMany<TSchema>(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading collection of unmapped {@TypeName} objects from path {@Path}", typeof(TSchema).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Load)
                                  .WithMetrics();

        var schemas = JsonSerializerEx.Deserialize<IEnumerable<TSchema>>(path, JsonSerializerOptions);

        if (schemas is null)
            throw new SerializationException($"Failed to serialize collection of unmapped {typeof(TSchema).Name} from path \"{path}\"");

        metricsLogger.LogTrace("Loaded collection of unmapped {@TypeName} objects from path {@Path}", typeof(TSchema).Name, path);

        return schemas;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<TSchema> LoadManyAsync<TSchema>(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Load)
              .LogTrace("Loading collection of unmapped {@TypeName} objects from path {@Path}", typeof(TSchema).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Load)
                                  .WithMetrics();

        var schemas = await JsonSerializerEx.DeserializeAsync<IEnumerable<TSchema>>(path, JsonSerializerOptions);

        if (schemas is null)
            throw new SerializationException($"Failed to serialize collection of unmapped {typeof(TSchema).Name} from path \"{path}\"");

        foreach (var schema in schemas)
            yield return schema;

        metricsLogger.LogTrace("Loaded collection of unmapped {@TypeName} objects from path {@Path}", typeof(TSchema).Name, path);
    }

    /// <inheritdoc />
    public void Save<TSchema>(TSchema obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving {@TypeName} to path {@Path}", typeof(TSchema).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Save)
                                  .WithMetrics();

        JsonSerializerEx.Serialize(
            path,
            obj,
            JsonSerializerOptions,
            Options.SafeSaves);

        metricsLogger.LogTrace("Saved {@TypeName} to path {@Path}", typeof(TSchema).Name, path);
    }

    /// <inheritdoc />
    public void SaveAndMap<T, TSchema>(T obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving {@TypeName} to path {@Path}", typeof(T).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Save)
                                  .WithMetrics();

        var schema = Mapper.Map<TSchema>(obj)!;

        JsonSerializerEx.Serialize(
            path,
            schema,
            JsonSerializerOptions,
            Options.SafeSaves);

        metricsLogger.LogTrace("Saved {@TypeName} to path {@Path}", typeof(T).Name, path);
    }

    /// <inheritdoc />
    public async Task SaveAndMapAsync<T, TSchema>(T obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving {@TypeName} to path {@Path}", typeof(T).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Save)
                                  .WithMetrics();

        var schema = Mapper.Map<TSchema>(obj)!;

        await JsonSerializerEx.SerializeAsync(
            path,
            schema,
            JsonSerializerOptions,
            Options.SafeSaves);

        metricsLogger.LogTrace("Saved {@TypeName} to path {@Path}", typeof(T).Name, path);
    }

    /// <inheritdoc />
    public void SaveAndMapMany<T, TSchema>(IEnumerable<T> obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving many {@TypeName} to path {@Path}", typeof(T).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Save)
                                  .WithMetrics();

        var schema = Mapper.MapMany<T, TSchema>(obj);

        JsonSerializerEx.Serialize(
            path,
            schema,
            JsonSerializerOptions,
            Options.SafeSaves);

        metricsLogger.LogTrace("Saved many {@TypeName} to path {@Path}", typeof(T).Name, path);
    }

    /// <inheritdoc />
    public async Task SaveAndMapManyAsync<T, TSchema>(IEnumerable<T> obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving many {@TypeName} to path {@Path}", typeof(T).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Save)
                                  .WithMetrics();

        var schema = Mapper.MapMany<T, TSchema>(obj);

        await JsonSerializerEx.SerializeAsync(
            path,
            schema,
            JsonSerializerOptions,
            Options.SafeSaves);

        metricsLogger.LogTrace("Saved many {@TypeName} to path {@Path}", typeof(T).Name, path);
    }

    /// <inheritdoc />
    public async Task SaveAsync<TSchema>(TSchema obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving {@TypeName} to path {@Path}", typeof(TSchema).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Save)
                                  .WithMetrics();

        await JsonSerializerEx.SerializeAsync(
            path,
            obj,
            JsonSerializerOptions,
            Options.SafeSaves);

        metricsLogger.LogTrace("Saved {@TypeName} to path {@Path}", typeof(TSchema).Name, path);
    }

    /// <inheritdoc />
    public void SaveMany<TSchema>(IEnumerable<TSchema> obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving many {@TypeName} to path {@Path}", typeof(TSchema).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Save)
                                  .WithMetrics();

        JsonSerializerEx.Serialize(
            path,
            obj,
            JsonSerializerOptions,
            Options.SafeSaves);

        metricsLogger.LogTrace("Saved many {@TypeName} to path {@Path}", typeof(TSchema).Name, path);
    }

    /// <inheritdoc />
    public async Task SaveManyAsync<TSchema>(IEnumerable<TSchema> obj, string path)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(path);

        Logger.WithTopics(Topics.Actions.Save)
              .LogTrace("Saving many {@TypeName} to path {@Path}", typeof(TSchema).Name, path);

        var metricsLogger = Logger.WithTopics(Topics.Actions.Save)
                                  .WithMetrics();

        await JsonSerializerEx.SerializeAsync(
            path,
            obj,
            JsonSerializerOptions,
            Options.SafeSaves);

        metricsLogger.LogTrace("Saved many {@TypeName} to path {@Path}", typeof(TSchema).Name, path);
    }
}