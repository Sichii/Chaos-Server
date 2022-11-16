using System.Text.Json;
using Chaos.Common.Synchronization;
using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Schemas.Aisling;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public sealed class UserSaveManager : ISaveManager<Aisling>
{
    private static readonly AutoReleasingSemaphoreSlim Sync = new(1, 1);
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger Logger;
    private readonly ITypeMapper Mapper;
    private readonly UserSaveManagerOptions Options;

    public UserSaveManager(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<UserSaveManagerOptions> options,
        ILogger<UserSaveManager> logger
    )
    {
        Options = options.Value;
        Mapper = mapper;
        Logger = logger;
        JsonSerializerOptions = jsonSerializerOptions.Value;

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    private async ValueTask<T> DesierlizeAsync<T>(string directory, string fileName)
    {
        var path = Path.Combine(directory, fileName);
        await using var stream = File.OpenRead(path);
        var ret = await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions);

        return ret!;
    }

    public async Task<Aisling> LoadAsync(string name)
    {
        await using var sync = await Sync.WaitAsync();

        Logger.LogTrace("Loading aisling {Name}", name);

        var directory = Path.Combine(Options.Directory, name.ToLower());

        var aislingSchema = await DesierlizeAsync<AislingSchema>(directory, "aisling.json");
        var bankSchema = await DesierlizeAsync<BankSchema>(directory, "bank.json");
        var effectsSchema = await DesierlizeAsync<EffectsBarSchema>(directory, "effects.json");
        var equipmentSchema = await DesierlizeAsync<EquipmentSchema>(directory, "equipment.json");
        var inventorySchema = await DesierlizeAsync<InventorySchema>(directory, "inventory.json");
        var skillsSchemas = await DesierlizeAsync<SkillBookSchema>(directory, "skills.json");
        var spellsSchemas = await DesierlizeAsync<SpellBookSchema>(directory, "spells.json");
        var legendSchema = await DesierlizeAsync<LegendSchema>(directory, "legend.json");

        var aisling = Mapper.Map<Aisling>(aislingSchema);
        var bank = Mapper.Map<Bank>(bankSchema);
        var effects = Mapper.MapMany<EffectSchema, IEffect>(effectsSchema);
        var equipment = Mapper.Map<Equipment>(equipmentSchema);
        var inventory = Mapper.Map<Inventory>(inventorySchema);
        var skillBook = Mapper.Map<SkillBook>(skillsSchemas);
        var spellBook = Mapper.Map<SpellBook>(spellsSchemas);
        var legend = Mapper.Map<Legend>(legendSchema);

        aisling.Initialize(
            name,
            bank,
            equipment,
            inventory,
            skillBook,
            spellBook,
            legend,
            effects);

        Logger.LogTrace("Loaded aisling {Name}", name);

        return aisling;
    }

    public async Task SaveAsync(Aisling aisling)
    {
        await using var sync = await Sync.WaitAsync();

        Logger.LogTrace("Saving aisling {Name}", aisling.Name);

        var aislingSchema = Mapper.Map<AislingSchema>(aisling);
        var bankSchema = Mapper.Map<BankSchema>(aisling.Bank);
        var equipmentSchema = Mapper.MapMany<ItemSchema>(aisling.Equipment).ToList();
        var inventorySchema = Mapper.MapMany<ItemSchema>(aisling.Inventory).ToList();
        var skillsSchemas = Mapper.MapMany<SkillSchema>(aisling.SkillBook).ToList();
        var spellsSchemas = Mapper.MapMany<SpellSchema>(aisling.SpellBook).ToList();
        var legendSchema = Mapper.MapMany<LegendMarkSchema>(aisling.Legend).ToList();
        var effectsSchemas = Mapper.MapMany<IEffect, EffectSchema>(aisling.Effects).ToList();

        var directory = Path.Combine(Options.Directory, aisling.Name.ToLower());

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await Task.WhenAll(
            SerializeAsync(directory, "aisling.json", aislingSchema),
            SerializeAsync(directory, "bank.json", bankSchema),
            SerializeAsync(directory, "effects.json", effectsSchemas),
            SerializeAsync(directory, "equipment.json", equipmentSchema),
            SerializeAsync(directory, "inventory.json", inventorySchema),
            SerializeAsync(directory, "skills.json", skillsSchemas),
            SerializeAsync(directory, "spells.json", spellsSchemas),
            SerializeAsync(directory, "legend.json", legendSchema));

        Logger.LogTrace("Saved aisling {Name}", aisling.Name);
    }

    private async Task SerializeAsync(string directory, string fileName, object value)
    {
        var path = Path.Combine(directory, fileName);
        await using var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
        await JsonSerializer.SerializeAsync(stream, value, JsonSerializerOptions);
    }
}