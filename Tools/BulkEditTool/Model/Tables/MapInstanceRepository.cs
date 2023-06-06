using System.IO;
using System.Text.Json;
using BulkEditTool.Model.Abstractions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace BulkEditTool.Model.Tables;

public sealed class MapInstanceRepository : RepositoryBase<MapInstanceRepository.MapInstanceComposite, MapInstanceCacheOptions>
{
    /// <inheritdoc />
    public MapInstanceRepository(IOptions<MapInstanceCacheOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }

    public override void Add(string path, MapInstanceComposite obj)
    {
        var wrapper = new TraceWrapper<MapInstanceComposite>(path, obj);
        Objects.Add(wrapper);
    }

    /// <inheritdoc />
    protected override async Task<MapInstanceComposite?> LoadFromFileAsync(string path)
    {
        try
        {
            var instanceTask = JsonSerializerEx.DeserializeAsync<MapInstanceSchema>(
                Path.Combine(path, "instance.json"),
                JsonSerializerOptions);

            var merchantsTask = JsonSerializerEx.DeserializeAsync<List<MerchantSpawnSchema>>(
                Path.Combine(path, "merchants.json"),
                JsonSerializerOptions);

            var monstersTask = JsonSerializerEx.DeserializeAsync<List<MonsterSpawnSchema>>(
                Path.Combine(path, "monsters.json"),
                JsonSerializerOptions);

            var reactorsTask = JsonSerializerEx.DeserializeAsync<List<ReactorTileSchema>>(
                Path.Combine(path, "reactors.json"),
                JsonSerializerOptions);

            await Task.WhenAll(
                instanceTask,
                merchantsTask,
                monstersTask,
                reactorsTask);

            if ((instanceTask.Result == null)
                || (merchantsTask.Result == null)
                || (monstersTask.Result == null)
                || (reactorsTask.Result == null))
                return null;

            return new MapInstanceComposite
            {
                Instance = instanceTask.Result,
                Merchants = merchantsTask.Result,
                Monsters = monstersTask.Result,
                Reactors = reactorsTask.Result
            };
        } catch (Exception e) //must be "Exception" because this will throw an AggregateException, not a JsonException
        {
            throw new JsonException($"Failed to deserialize {nameof(MapInstanceComposite)} from path \"{path}\"", e);
        }
    }

    public override void Remove(string name)
    {
        var wrapper = Objects.FirstOrDefault(wp => wp.Obj.Instance.InstanceId.EqualsI(name));

        if (wrapper is null)
            return;

        Directory.Delete(wrapper.Path, true);
        Objects.Remove(wrapper);
    }

    public override async Task SaveItemAsync(TraceWrapper<MapInstanceComposite> wrapped)
    {
        try
        {
            if (!Directory.Exists(wrapped.Path))
                Directory.CreateDirectory(wrapped.Path);

            await Task.WhenAll(
                JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "instance.json"), wrapped.Obj.Instance, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "merchants.json"), wrapped.Obj.Merchants, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "monsters.json"), wrapped.Obj.Monsters, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "reactors.json"), wrapped.Obj.Reactors, JsonSerializerOptions));
        } catch (Exception e) //must be "Exception" because this will throw an AggregateException, not a JsonException
        {
            throw new JsonException($"Failed to serialize {nameof(MapInstanceComposite)} to path \"{wrapped.Path}\"", e);
        }
    }

    public sealed class MapInstanceComposite
    {
        public required MapInstanceSchema Instance { get; init; }
        public required List<MerchantSpawnSchema> Merchants { get; init; }
        public required List<MonsterSpawnSchema> Monsters { get; init; }
        public required List<ReactorTileSchema> Reactors { get; init; }
    }
}