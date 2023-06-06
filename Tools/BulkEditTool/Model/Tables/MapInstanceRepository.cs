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
        var instance = await JsonSerializerEx.DeserializeAsync<MapInstanceSchema>(
            Path.Combine(path, "instance.json"),
            JsonSerializerOptions);

        var merchants = await JsonSerializerEx.DeserializeAsync<List<MerchantSpawnSchema>>(
            Path.Combine(path, "merchants.json"),
            JsonSerializerOptions);

        var monsters = await JsonSerializerEx.DeserializeAsync<List<MonsterSpawnSchema>>(
            Path.Combine(path, "monsters.json"),
            JsonSerializerOptions);

        var reactors = await JsonSerializerEx.DeserializeAsync<List<ReactorTileSchema>>(
            Path.Combine(path, "reactors.json"),
            JsonSerializerOptions);

        if ((instance == null) || (merchants == null) || (monsters == null) || (reactors == null))
            return null;

        return new MapInstanceComposite
        {
            Instance = instance,
            Merchants = merchants,
            Monsters = monsters,
            Reactors = reactors
        };
    }

    public override void Remove(string name)
    {
        var wrapper = Objects.FirstOrDefault(wp => wp.Obj.Instance.InstanceId.EqualsI(name));

        if (wrapper is null)
            return;

        Directory.Delete(wrapper.Path, true);
        Objects.Remove(wrapper);
    }

    public override Task SaveItemAsync(TraceWrapper<MapInstanceComposite> wrapped)
    {
        if (!Directory.Exists(wrapped.Path))
            Directory.CreateDirectory(wrapped.Path);

        return Task.WhenAll(
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "instance.json"), wrapped.Obj.Instance, JsonSerializerOptions),
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "merchants.json"), wrapped.Obj.Merchants, JsonSerializerOptions),
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "monsters.json"), wrapped.Obj.Monsters, JsonSerializerOptions),
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "reactors.json"), wrapped.Obj.Reactors, JsonSerializerOptions));
    }

    public sealed class MapInstanceComposite
    {
        public required MapInstanceSchema Instance { get; init; }
        public required List<MerchantSpawnSchema> Merchants { get; init; }
        public required List<MonsterSpawnSchema> Monsters { get; init; }
        public required List<ReactorTileSchema> Reactors { get; init; }
    }
}