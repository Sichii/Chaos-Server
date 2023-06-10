using System.IO;
using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Schemas.Aisling;
using Chaos.Services.Storage.Options;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class AislingRepository : RepositoryBase<AislingRepository.AislingComposite>
{
    private new readonly AislingStoreOptions Options;

    /// <inheritdoc />
    public override string RootDirectory => Options.Directory;

    /// <inheritdoc />
    public AislingRepository(IOptions<AislingStoreOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(null, jsonSerializerOptions) => Options = options.Value;

    public override void Add(string path, AislingComposite obj)
    {
        var wrapper = new TraceWrapper<AislingComposite>(path, obj);
        Objects.Add(wrapper);
    }

    /// <inheritdoc />
    /// <remarks>Must override here because AislingStoreOptions is not an IExpiringFileCacheOptions implementation</remarks>
    protected override IEnumerable<string> GetPaths() =>
        Directory.EnumerateDirectories(Options.Directory, "*", SearchOption.AllDirectories)
                 .Where(src => Directory.EnumerateFiles(src).Any());

    /// <inheritdoc />
    protected override async Task<AislingComposite?> LoadFromFileAsync(string path)
    {
        try
        {
            var aislingTask = JsonSerializerEx.DeserializeAsync<AislingSchema>(Path.Combine(path, "aisling.json"), JsonSerializerOptions);
            var bankTask = JsonSerializerEx.DeserializeAsync<BankSchema>(Path.Combine(path, "bank.json"), JsonSerializerOptions);

            var effectsTask = JsonSerializerEx.DeserializeAsync<List<EffectSchema>>(
                Path.Combine(path, "effects.json"),
                JsonSerializerOptions);

            var equipmentTask = JsonSerializerEx.DeserializeAsync<List<ItemSchema>>(
                Path.Combine(path, "equipment.json"),
                JsonSerializerOptions);

            var inventoryTask = JsonSerializerEx.DeserializeAsync<List<ItemSchema>>(
                Path.Combine(path, "inventory.json"),
                JsonSerializerOptions);

            var legendTask = JsonSerializerEx.DeserializeAsync<List<LegendMarkSchema>>(
                Path.Combine(path, "legend.json"),
                JsonSerializerOptions);

            var skillsTask = JsonSerializerEx.DeserializeAsync<List<SkillSchema>>(Path.Combine(path, "skills.json"), JsonSerializerOptions);
            var spellsTask = JsonSerializerEx.DeserializeAsync<List<SpellSchema>>(Path.Combine(path, "spells.json"), JsonSerializerOptions);

            var trackersTask =
                JsonSerializerEx.DeserializeAsync<TrackersSchema>(Path.Combine(path, "trackers.json"), JsonSerializerOptions);

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

            if ((aislingTask.Result == null)
                || (bankTask.Result == null)
                || (effectsTask.Result == null)
                || (equipmentTask.Result == null)
                || (inventoryTask.Result == null)
                || (legendTask.Result == null)
                || (skillsTask.Result == null)
                || (spellsTask.Result == null)
                || (trackersTask.Result == null))
                return null;

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
                JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "aisling.json"), wrapped.Object.Aisling, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "bank.json"), wrapped.Object.Bank, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "effects.json"), wrapped.Object.Effects, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(
                    Path.Combine(wrapped.Path, "equipment.json"),
                    wrapped.Object.Equipment,
                    JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(
                    Path.Combine(wrapped.Path, "inventory.json"),
                    wrapped.Object.Inventory,
                    JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "legend.json"), wrapped.Object.Legend, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "skills.json"), wrapped.Object.Skills, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "spells.json"), wrapped.Object.Spells, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(
                    Path.Combine(wrapped.Path, "trackers.json"),
                    wrapped.Object.Trackers,
                    JsonSerializerOptions));
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