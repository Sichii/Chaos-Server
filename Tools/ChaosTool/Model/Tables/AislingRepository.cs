#region
using System.IO;
using System.Text.Json;
using Chaos.Extensions.Common;
using Chaos.Schemas.Aisling;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;
#endregion

namespace ChaosTool.Model.Tables;

public sealed class AislingRepository : RepositoryBase<AislingRepository.AislingComposite>
{
    private new readonly AislingStoreOptions Options;

    /// <inheritdoc />
    public override string RootDirectory => Options.Directory;

    /// <inheritdoc />
    public AislingRepository(IEntityRepository entityRepository, IOptions<AislingStoreOptions> options)
        : base(entityRepository, null)
        => Options = options.Value;

    /// <inheritdoc />
    /// <remarks>
    ///     Must override here because AislingStoreOptions is not an IExpiringFileCacheOptions implementation
    /// </remarks>
    protected override IEnumerable<string> GetPaths()
        => Directory.EnumerateDirectories(Options.Directory, "*", SearchOption.AllDirectories)
                    .Where(src => Directory.EnumerateFiles(src)
                                           .Any());

    /// <inheritdoc />
    protected override async Task<AislingComposite?> LoadFromFileAsync(string path)
    {
        try
        {
            var aislingTask = EntityRepository.LoadAsync<AislingSchema>(Path.Combine(path, "aisling.json"));
            var bankTask = EntityRepository.LoadAsync<BankSchema>(Path.Combine(path, "bank.json"));

            var effectsTask = EntityRepository.LoadManyAsync<EffectSchema>(Path.Combine(path, "effects.json"))
                                              .ToListAsync()
                                              .AsTask();

            var equipmentTask = EntityRepository.LoadManyAsync<ItemSchema>(Path.Combine(path, "equipment.json"))
                                                .ToListAsync()
                                                .AsTask();

            var inventoryTask = EntityRepository.LoadManyAsync<ItemSchema>(Path.Combine(path, "inventory.json"))
                                                .ToListAsync()
                                                .AsTask();

            var legendTask = EntityRepository.LoadManyAsync<LegendMarkSchema>(Path.Combine(path, "legend.json"))
                                             .ToListAsync()
                                             .AsTask();

            var skillsTask = EntityRepository.LoadManyAsync<SkillSchema>(Path.Combine(path, "skills.json"))
                                             .ToListAsync()
                                             .AsTask();

            var spellsTask = EntityRepository.LoadManyAsync<SpellSchema>(Path.Combine(path, "spells.json"))
                                             .ToListAsync()
                                             .AsTask();

            var trackersTask = EntityRepository.LoadAsync<AislingTrackersSchema>(Path.Combine(path, "trackers.json"));

            await Task.WhenAll(
                aislingTask,
                bankTask,
                effectsTask,
                equipmentTask,
                inventoryTask,
                legendTask,
                skillsTask,
                spellsTask,
                trackersTask);

            return new AislingComposite
            {
                Aisling = await aislingTask,
                Bank = await bankTask,
                Effects = await effectsTask,
                Equipment = await equipmentTask,
                Inventory = await inventoryTask,
                Legend = await legendTask,
                Skills = await skillsTask,
                Spells = await spellsTask,
                Trackers = await trackersTask
            };
        } catch (Exception e) //must be "Exception" because this will throw an AggregateException, not a JsonException
        {
            throw new JsonException($"Failed to deserialize {nameof(AislingComposite)} from path \"{path}\"", e);
        }
    }

    public override void Remove(string originalPath)
    {
        var wrapped = Objects.FirstOrDefault(wp => wp.Path.EqualsI(originalPath));

        if (wrapped is null)
            return;

        Directory.Delete(wrapped.Path, true);
        Objects.Remove(wrapped);
    }

    public override async Task SaveItemAsync(TraceWrapper<AislingComposite> wrapped)
    {
        try
        {
            if (!Directory.Exists(wrapped.Path))
                Directory.CreateDirectory(wrapped.Path);

            await Task.WhenAll(
                EntityRepository.SaveAsync(wrapped.Object.Aisling, Path.Combine(wrapped.Path, "aisling.json")),
                EntityRepository.SaveAsync(wrapped.Object.Bank, Path.Combine(wrapped.Path, "bank.json")),
                EntityRepository.SaveManyAsync(wrapped.Object.Effects, Path.Combine(wrapped.Path, "effects.json")),
                EntityRepository.SaveManyAsync(wrapped.Object.Equipment, Path.Combine(wrapped.Path, "equipment.json")),
                EntityRepository.SaveManyAsync(wrapped.Object.Inventory, Path.Combine(wrapped.Path, "inventory.json")),
                EntityRepository.SaveManyAsync(wrapped.Object.Legend, Path.Combine(wrapped.Path, "legend.json")),
                EntityRepository.SaveManyAsync(wrapped.Object.Skills, Path.Combine(wrapped.Path, "skills.json")),
                EntityRepository.SaveManyAsync(wrapped.Object.Spells, Path.Combine(wrapped.Path, "spells.json")),
                EntityRepository.SaveAsync(wrapped.Object.Trackers, Path.Combine(wrapped.Path, "trackers.json")));
        } catch (Exception e) //must be "Exception" because this will throw an AggregateException, not a JsonException
        {
            throw new JsonException($"Failed to serialize {nameof(AislingComposite)} to path \"{wrapped.Path}\"", e);
        }
    }

    public sealed class AislingComposite
    {
        public required AislingSchema Aisling { get; init; }
        public required BankSchema Bank { get; init; }
        public required ICollection<EffectSchema> Effects { get; init; }
        public required ICollection<ItemSchema> Equipment { get; init; }
        public required ICollection<ItemSchema> Inventory { get; init; }
        public required ICollection<LegendMarkSchema> Legend { get; init; }
        public required ICollection<SkillSchema> Skills { get; init; }
        public required ICollection<SpellSchema> Spells { get; init; }
        public required AislingTrackersSchema Trackers { get; init; }
    }
}