#region
using System.IO;
using System.Text.Json;
using Chaos.Extensions.Common;
using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;
#endregion

namespace ChaosTool.Model.Tables;

public sealed class MapInstanceRepository : RepositoryBase<MapInstanceRepository.MapInstanceComposite>
{
    /// <inheritdoc />
    public MapInstanceRepository(IEntityRepository entityRepository, IOptions<MapInstanceCacheOptions> options)
        : base(entityRepository, options) { }

    /// <inheritdoc />
    protected override async Task<MapInstanceComposite?> LoadFromFileAsync(string path)
    {
        try
        {
            var instanceTask = EntityRepository.LoadAsync<MapInstanceSchema>(Path.Combine(path, "instance.json"));

            var merchantsTask = EntityRepository.LoadManyAsync<MerchantSpawnSchema>(Path.Combine(path, "merchants.json"))
                                                .ToListAsync()
                                                .AsTask();

            var monstersTask = EntityRepository.LoadManyAsync<MonsterSpawnSchema>(Path.Combine(path, "monsters.json"))
                                               .ToListAsync()
                                               .AsTask();

            var reactorsTask = EntityRepository.LoadManyAsync<ReactorTileSchema>(Path.Combine(path, "reactors.json"))
                                               .ToListAsync()
                                               .AsTask();

            await Task.WhenAll(
                instanceTask,
                merchantsTask,
                monstersTask,
                reactorsTask);

            return new MapInstanceComposite
            {
                Instance = await instanceTask,
                Merchants = await merchantsTask,
                Monsters = await monstersTask,
                Reactors = await reactorsTask
            };
        } catch (Exception e) //must be "Exception" because this will throw an AggregateException, not a JsonException
        {
            throw new JsonException($"Failed to deserialize {nameof(MapInstanceComposite)} from path \"{path}\"", e);
        }
    }

    public override void Remove(string originalPath)
    {
        var wrapper = Objects.FirstOrDefault(wp => wp.Object.Instance.InstanceId.EqualsI(originalPath));

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
                EntityRepository.SaveAsync(wrapped.Object.Instance, Path.Combine(wrapped.Path, "instance.json")),
                EntityRepository.SaveManyAsync(wrapped.Object.Merchants, Path.Combine(wrapped.Path, "merchants.json")),
                EntityRepository.SaveManyAsync(wrapped.Object.Monsters, Path.Combine(wrapped.Path, "monsters.json")),
                EntityRepository.SaveManyAsync(wrapped.Object.Reactors, Path.Combine(wrapped.Path, "reactors.json")));
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