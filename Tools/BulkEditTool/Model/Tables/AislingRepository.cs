using System.IO;
using System.Text.Json;
using BulkEditTool.Model.Abstractions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Schemas.Aisling;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace BulkEditTool.Model.Tables;

public sealed class AislingRepository : RepositoryBase<AislingRepository.AislingComposite, AislingStoreOptions>
{
    /// <inheritdoc />
    public AislingRepository(IOptions<AislingStoreOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }

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
        var aisling = await JsonSerializerEx.DeserializeAsync<AislingSchema>(Path.Combine(path, "aisling.json"), JsonSerializerOptions);
        var bank = await JsonSerializerEx.DeserializeAsync<BankSchema>(Path.Combine(path, "bank.json"), JsonSerializerOptions);

        var effects =
            await JsonSerializerEx.DeserializeAsync<List<EffectSchema>>(Path.Combine(path, "effects.json"), JsonSerializerOptions);

        var equipment = await JsonSerializerEx.DeserializeAsync<List<ItemSchema>>(
            Path.Combine(path, "equipment.json"),
            JsonSerializerOptions);

        var inventory = await JsonSerializerEx.DeserializeAsync<List<ItemSchema>>(
            Path.Combine(path, "inventory.json"),
            JsonSerializerOptions);

        var legend = await JsonSerializerEx.DeserializeAsync<List<LegendMarkSchema>>(
            Path.Combine(path, "legend.json"),
            JsonSerializerOptions);

        var skills = await JsonSerializerEx.DeserializeAsync<List<SkillSchema>>(Path.Combine(path, "skills.json"), JsonSerializerOptions);
        var spells = await JsonSerializerEx.DeserializeAsync<List<SpellSchema>>(Path.Combine(path, "spells.json"), JsonSerializerOptions);
        var trackers = await JsonSerializerEx.DeserializeAsync<TrackersSchema>(Path.Combine(path, "trackers.json"), JsonSerializerOptions);

        if ((aisling == null)
            || (bank == null)
            || (effects == null)
            || (equipment == null)
            || (inventory == null)
            || (legend == null)
            || (skills == null)
            || (spells == null)
            || (trackers == null))
            return null;

        return new AislingComposite
        {
            Aisling = aisling,
            Bank = bank,
            Effects = effects,
            Equipment = equipment,
            Inventory = inventory,
            Legend = legend,
            Skills = skills,
            Spells = spells,
            Trackers = trackers
        };
    }

    public override void Remove(string name)
    {
        var wrapper = Objects.FirstOrDefault(wp => wp.Obj.Aisling.Name.EqualsI(name));

        if (wrapper is null)
            return;

        Directory.Delete(wrapper.Path, true);
        Objects.Remove(wrapper);
    }

    public override Task SaveItemAsync(TraceWrapper<AislingComposite> wrapped)
    {
        if (!Directory.Exists(wrapped.Path))
            Directory.CreateDirectory(wrapped.Path);

        return Task.WhenAll(
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "aisling.json"), wrapped.Obj.Aisling, JsonSerializerOptions),
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "bank.json"), wrapped.Obj.Bank, JsonSerializerOptions),
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "effects.json"), wrapped.Obj.Effects, JsonSerializerOptions),
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "equipment.json"), wrapped.Obj.Equipment, JsonSerializerOptions),
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "inventory.json"), wrapped.Obj.Inventory, JsonSerializerOptions),
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "legend.json"), wrapped.Obj.Legend, JsonSerializerOptions),
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "skills.json"), wrapped.Obj.Skills, JsonSerializerOptions),
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "spells.json"), wrapped.Obj.Spells, JsonSerializerOptions),
            JsonSerializerEx.SerializeAsync(Path.Combine(wrapped.Path, "trackers.json"), wrapped.Obj.Trackers, JsonSerializerOptions));
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