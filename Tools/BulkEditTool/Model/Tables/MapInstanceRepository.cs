using System.IO;
using System.Text.Json;
using BulkEditTool.Model.Abstractions;
using Chaos.Common.Utilities;
using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace BulkEditTool.Model.Tables;

public sealed class MapInstanceRepository : RepositoryBase<MapInstanceRepository.MapInstanceComposite, MapInstanceCacheOptions>
{
    /// <inheritdoc />
    public MapInstanceRepository(IOptions<MapInstanceCacheOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }

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

    internal override async Task SaveChangesAsync()
    {
        foreach (var obj in Objects)
        {
            await JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "instance.json"), obj.Obj.Instance, JsonSerializerOptions);
            await JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "merchants.json"), obj.Obj.Merchants, JsonSerializerOptions);
            await JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "monsters.json"), obj.Obj.Monsters, JsonSerializerOptions);
            await JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "reactors.json"), obj.Obj.Reactors, JsonSerializerOptions);
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