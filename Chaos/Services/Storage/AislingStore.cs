using System.Diagnostics;
using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.IO.FileSystem;
using Chaos.Models.Legend;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

/// <summary>
///     Manages save files for Aislings
/// </summary>
public sealed class AislingStore : IAsyncStore<Aisling>
{
    private readonly IEntityRepository EntityRepository;
    private readonly ICloningService<Item> ItemCloningService;
    private readonly ILogger<AislingStore> Logger;
    private readonly AislingStoreOptions Options;

    public AislingStore(
        IEntityRepository entityRepository,
        IOptions<AislingStoreOptions> options,
        ILogger<AislingStore> logger,
        ICloningService<Item> itemCloningService
    )
    {
        Logger = logger;
        Options = options.Value;
        ItemCloningService = itemCloningService;
        EntityRepository = entityRepository;
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(string key)
    {
        var directory = Path.Combine(Options.Directory, key.ToLower());

        return Task.FromResult(Directory.Exists(directory));
    }

    public async Task<Aisling> LoadAsync(string name)
    {
        Logger.LogTrace("Loading aisling {@AislingName}", name);

        var directory = Path.Combine(Options.Directory, name.ToLower());

        var aisling = await directory.SafeExecuteAsync(dir => InnerLoadAsync(name, dir));

        Logger.WithProperty(aisling)
              .LogDebug("Loaded aisling {@AislingName}", aisling.Name);

        return aisling;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key) =>
        throw new NotImplementedException("This would effectively delete the character. This is reserved for manual operations");

    public async Task SaveAsync(Aisling aisling)
    {
        Logger.WithProperty(aisling)
              .LogTrace("Saving {@AislingName}", aisling.Name);

        var start = Stopwatch.GetTimestamp();

        try
        {
            var directory = Path.Combine(Options.Directory, aisling.Name.ToLower());

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await directory.SafeExecuteAsync(dir => InnerSaveAsync(dir, aisling));
            Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);

            Logger.WithProperty(aisling)
                  .LogDebug("Saved aisling {@AislingName}, took {@Elapsed}", aisling.Name, Stopwatch.GetElapsedTime(start));
        } catch (Exception e)
        {
            Logger.WithProperty(aisling)
                  .LogCritical(
                      e,
                      "Failed to save aisling {@AislingName} in {@Elapsed}",
                      aisling.Name,
                      Stopwatch.GetElapsedTime(start));
        }
    }

    private async Task<Aisling> InnerLoadAsync(string name, string directory)
    {
        if (!Directory.Exists(directory))
            throw new InvalidOperationException($"No aisling data exists for the key \"{name}\" at the specified path \"{directory}\"");

        var aislingPath = Path.Combine(directory, "aisling.json");
        var bankPath = Path.Combine(directory, "bank.json");
        var trackersPath = Path.Combine(directory, "trackers.json");
        var legendPath = Path.Combine(directory, "legend.json");
        var inventoryPath = Path.Combine(directory, "inventory.json");
        var skillsPath = Path.Combine(directory, "skills.json");
        var spellsPath = Path.Combine(directory, "spells.json");
        var equipmentPath = Path.Combine(directory, "equipment.json");
        var effectsPath = Path.Combine(directory, "effects.json");

        var aislingTask = EntityRepository.LoadAndMapAsync<Aisling, AislingSchema>(aislingPath);
        var bankTask = EntityRepository.LoadAndMapAsync<Bank, BankSchema>(bankPath);
        var trackersTask = EntityRepository.LoadAndMapAsync<AislingTrackers, TrackersSchema>(trackersPath);
        var effectsTask = EntityRepository.LoadAndMapManyAsync<IEffect, EffectSchema>(effectsPath).ToListAsync();
        var equipmentTask = EntityRepository.LoadAndMapManyAsync<Item, ItemSchema>(equipmentPath).ToListAsync();
        var inventoryTask = EntityRepository.LoadAndMapManyAsync<Item, ItemSchema>(inventoryPath).ToListAsync();
        var skillsTask = EntityRepository.LoadAndMapManyAsync<Skill, SkillSchema>(skillsPath).ToListAsync();
        var spellsTask = EntityRepository.LoadAndMapManyAsync<Spell, SpellSchema>(spellsPath).ToListAsync();
        var legendTask = EntityRepository.LoadAndMapManyAsync<LegendMark, LegendMarkSchema>(legendPath).ToListAsync();

        var aisling = await aislingTask;
        var bank = await bankTask;
        var trackers = await trackersTask;

        var effectsBar = new EffectsBar(aisling, await effectsTask);
        var equipment = new Equipment(await equipmentTask);
        var inventory = new Inventory(ItemCloningService, await inventoryTask);
        var skillBook = new SkillBook(await skillsTask);
        var spellBook = new SpellBook(await spellsTask);
        var legend = new Legend(await legendTask);

        aisling.Initialize(
            name,
            bank,
            equipment,
            inventory,
            skillBook,
            spellBook,
            legend,
            effectsBar,
            trackers);

        return aisling;
    }

    private Task InnerSaveAsync(string directory, Aisling aisling)
    {
        var aislingPath = Path.Combine(directory, "aisling.json");
        var bankPath = Path.Combine(directory, "bank.json");
        var trackersPath = Path.Combine(directory, "trackers.json");
        var legendPath = Path.Combine(directory, "legend.json");
        var inventoryPath = Path.Combine(directory, "inventory.json");
        var skillsPath = Path.Combine(directory, "skills.json");
        var spellsPath = Path.Combine(directory, "spells.json");
        var equipmentPath = Path.Combine(directory, "equipment.json");
        var effectsPath = Path.Combine(directory, "effects.json");

        return Task.WhenAll(
            EntityRepository.SaveAndMapAsync<Aisling, AislingSchema>(aisling, aislingPath),
            EntityRepository.SaveAndMapAsync<Bank, BankSchema>(aisling.Bank, bankPath),
            EntityRepository.SaveAndMapAsync<Trackers, TrackersSchema>(aisling.Trackers, trackersPath),
            EntityRepository.SaveAndMapManyAsync<LegendMark, LegendMarkSchema>(aisling.Legend, legendPath),
            EntityRepository.SaveAndMapManyAsync<Item, ItemSchema>(aisling.Inventory, inventoryPath),
            EntityRepository.SaveAndMapManyAsync<Skill, SkillSchema>(aisling.SkillBook, skillsPath),
            EntityRepository.SaveAndMapManyAsync<Spell, SpellSchema>(aisling.SpellBook, spellsPath),
            EntityRepository.SaveAndMapManyAsync<Item, ItemSchema>(aisling.Equipment, equipmentPath),
            EntityRepository.SaveAndMapManyAsync<IEffect, EffectSchema>(aisling.Effects, effectsPath));
    }
}