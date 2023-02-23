using System.Diagnostics;
using System.Text.Json;
using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public sealed class UserSaveManager : ISaveManager<Aisling>
{
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly ILogger<UserSaveManager> Logger;
    private readonly ITypeMapper Mapper;
    private readonly UserSaveManagerOptions Options;

    public UserSaveManager(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<UserSaveManagerOptions> options,
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

    private async ValueTask<T> DeserializeAsync<T>(string directory, string fileName)
    {
        var path = Path.Combine(directory, fileName);

        await using var stream = File.Open(
            path,
            new FileStreamOptions
            {
                Access = FileAccess.Read,
                Mode = FileMode.Open,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
                Share = FileShare.Read
            });

        var ret = await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions);

        return ret!;
    }

    public async Task<Aisling> LoadAsync(string name)
    {
        Logger.LogTrace("Loading aisling {Name}", name);

        var directory = Path.Combine(Options.Directory, name.ToLower());

        var aislingSchema = await DeserializeAsync<AislingSchema>(directory, "aisling.json");
        var bankSchema = await DeserializeAsync<BankSchema>(directory, "bank.json");
        var effectsSchema = await DeserializeAsync<EffectsBarSchema>(directory, "effects.json");
        var equipmentSchema = await DeserializeAsync<EquipmentSchema>(directory, "equipment.json");
        var inventorySchema = await DeserializeAsync<InventorySchema>(directory, "inventory.json");
        var skillsSchemas = await DeserializeAsync<SkillBookSchema>(directory, "skills.json");
        var spellsSchemas = await DeserializeAsync<SpellBookSchema>(directory, "spells.json");
        var legendSchema = await DeserializeAsync<LegendSchema>(directory, "legend.json");
        var timedEventsSchema = await DeserializeAsync<TimedEventCollectionSchema>(directory, "timedEvents.json");

        var aisling = Mapper.Map<Aisling>(aislingSchema);
        var bank = Mapper.Map<Bank>(bankSchema);
        var effects = new EffectsBar(aisling, Mapper.MapMany<EffectSchema, IEffect>(effectsSchema));
        var equipment = Mapper.Map<Equipment>(equipmentSchema);
        var inventory = Mapper.Map<Inventory>(inventorySchema);
        var skillBook = Mapper.Map<SkillBook>(skillsSchemas);
        var spellBook = Mapper.Map<SpellBook>(spellsSchemas);
        var legend = Mapper.Map<Legend>(legendSchema);
        var timedEvents = Mapper.Map<TimedEventCollection>(timedEventsSchema);

        aisling.Initialize(
            name,
            bank,
            equipment,
            inventory,
            skillBook,
            spellBook,
            legend,
            effects,
            timedEvents);

        Logger.LogDebug("Loaded {@Player}", aisling);

        return aisling;
    }

    public async Task SaveAsync(Aisling obj)
    {
        Logger.LogTrace("Saving {@Player}", obj);
        var start = Stopwatch.GetTimestamp();

        try
        {
            var aislingSchema = Mapper.Map<AislingSchema>(obj);
            var bankSchema = Mapper.Map<BankSchema>(obj.Bank);
            var equipmentSchema = Mapper.MapMany<ItemSchema>(obj.Equipment).ToList();
            var inventorySchema = Mapper.MapMany<ItemSchema>(obj.Inventory).ToList();
            var effectsSchemas = Mapper.MapMany<IEffect, EffectSchema>(obj.Effects).ToList();
            var skillsSchemas = Mapper.MapMany<SkillSchema>(obj.SkillBook).ToList();
            var spellsSchemas = Mapper.MapMany<SpellSchema>(obj.SpellBook).ToList();
            var legendSchema = Mapper.MapMany<LegendMarkSchema>(obj.Legend).ToList();
            var timedEventsSchemas = Mapper.Map<TimedEventCollectionSchema>(obj.TimedEvents);

            var directory = Path.Combine(Options.Directory, obj.Name.ToLower());

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
                SerializeAsync(directory, "legend.json", legendSchema),
                SerializeAsync(directory, "timedEvents.json", timedEventsSchemas));

            Logger.LogDebug("Saved {@Player} in {Elapsed}", obj, Stopwatch.GetElapsedTime(start));
        } catch (Exception e)
        {
            Logger.LogError(
                e,
                "Failed to save aisling {@Player} in {Elapsed}",
                obj,
                Stopwatch.GetElapsedTime(start));
        }
    }

    private async Task SerializeAsync(string directory, string fileName, object value)
    {
        var path = Path.Combine(directory, fileName);
        var start = Stopwatch.GetTimestamp();

        await using var stream = File.Open(
            path,
            new FileStreamOptions
            {
                Access = FileAccess.ReadWrite,
                Mode = FileMode.OpenOrCreate,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
                Share = FileShare.None
            });

        stream.SetLength(0);

        await JsonSerializer.SerializeAsync(stream, value, JsonSerializerOptions);

        Logger.LogTrace("Serialized \"{FileName}\" in {Elapsed}", fileName, Stopwatch.GetElapsedTime(start));
    }
}