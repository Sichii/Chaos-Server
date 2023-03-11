using System.IO;
using System.Text.Json;
using BulkEditTool.Model.Abstractions;
using Chaos.Common.Utilities;
using Chaos.Schemas.Aisling;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Options;

namespace BulkEditTool.Model.Tables;

public sealed class AislingRepository : RepositoryBase<AislingRepository.AislingComposite, UserSaveManagerOptions>
{
    /// <inheritdoc />
    public AislingRepository(IOptions<UserSaveManagerOptions> options, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        : base(options, jsonSerializerOptions) { }

    /// <inheritdoc />
    protected override IEnumerable<string> GetPaths() =>
        Directory.EnumerateDirectories(Options.Directory, "*", SearchOption.AllDirectories)
                 .Where(src => Directory.EnumerateFiles(src).Any());

    /// <inheritdoc />
    protected override async Task<AislingComposite?> LoadFromFileAsync(string path)
    {
        var aisling = await JsonSerializerEx.DeserializeAsync<AislingSchema>(Path.Combine(path, "aisling.json"), JsonSerializerOptions);
        var bank = await JsonSerializerEx.DeserializeAsync<BankSchema>(Path.Combine(path, "bank.json"), JsonSerializerOptions);
        var effects = await JsonSerializerEx.DeserializeAsync<EffectsBarSchema>(Path.Combine(path, "effects.json"), JsonSerializerOptions);

        var equipment = await JsonSerializerEx.DeserializeAsync<EquipmentSchema>(
            Path.Combine(path, "equipment.json"),
            JsonSerializerOptions);

        var inventory = await JsonSerializerEx.DeserializeAsync<InventorySchema>(
            Path.Combine(path, "inventory.json"),
            JsonSerializerOptions);

        var legend = await JsonSerializerEx.DeserializeAsync<LegendSchema>(Path.Combine(path, "legend.json"), JsonSerializerOptions);
        var skills = await JsonSerializerEx.DeserializeAsync<SkillBookSchema>(Path.Combine(path, "skills.json"), JsonSerializerOptions);
        var spells = await JsonSerializerEx.DeserializeAsync<SpellBookSchema>(Path.Combine(path, "spells.json"), JsonSerializerOptions);
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

    internal override async Task SaveChangesAsync()
    {
        foreach (var obj in Objects)
            await Task.WhenAll(
                JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "aisling.json"), obj.Obj.Aisling, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "bank.json"), obj.Obj.Bank, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "effects.json"), obj.Obj.Effects, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "equipment.json"), obj.Obj.Equipment, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "inventory.json"), obj.Obj.Inventory, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "legend.json"), obj.Obj.Legend, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "skills.json"), obj.Obj.Skills, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "spells.json"), obj.Obj.Spells, JsonSerializerOptions),
                JsonSerializerEx.SerializeAsync(Path.Combine(obj.Path, "trackers.json"), obj.Obj.Trackers, JsonSerializerOptions));
    }

    public sealed class AislingComposite
    {
        public required AislingSchema Aisling { get; init; }
        public required BankSchema Bank { get; init; }
        public required EffectsBarSchema Effects { get; init; }
        public required EquipmentSchema Equipment { get; init; }
        public required InventorySchema Inventory { get; init; }
        public required LegendSchema Legend { get; init; }
        public required SkillBookSchema Skills { get; init; }
        public required SpellBookSchema Spells { get; init; }
        public required TrackersSchema Trackers { get; init; }
    }
}