using System.IO;
using System.Text.Json;
using Chaos.Extensions.Common;
using Chaos.Schemas.Aisling;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

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

    public override void Add(string path, AislingComposite obj)
    {
        var wrapper = new TraceWrapper<AislingComposite>(path, obj);
        Objects.Add(wrapper);
    }

    /// <inheritdoc />
    /// <remarks>Must override here because AislingStoreOptions is not an IExpiringFileCacheOptions implementation</remarks>
    protected override IEnumerable<string> GetPaths()
        => Directory.EnumerateDirectories(Options.Directory, "*", SearchOption.AllDirectories)
                    .Where(
                        src => Directory.EnumerateFiles(src)
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

            var trackersTask = EntityRepository.LoadAsync<TrackersSchema>(Path.Combine(path, "trackers.json"));

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
                Aisling = aislingTask.Result,
                Bank = bankTask.Result,
                Effects = effectsTask.Result,
                Equipment = equipmentTask.Result,
                Inventory = inventoryTask.Result,
                Legend = legendTask.Result,
                Skills = skillsTask.Result,
                Spells = spellsTask.Result,
                Trackers = trackersTask.Result
            };
        } catch (Exception e) //must be "Exception" because this will throw an AggregateException, not a JsonException
        {
            throw new JsonException($"Failed to deserialize {nameof(AislingComposite)} from path \"{path}\"", e);
        }
    }

    public override void Remove(string name)
    {
        var wrapper = Objects.FirstOrDefault(wp => wp.Object.Aisling.Name.EqualsI(name));

        if (wrapper is null)
            return;

        Directory.Delete(wrapper.Path, true);
        Objects.Remove(wrapper);
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
        public required TrackersSchema Trackers { get; init; }
    }
}