using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using Chaos.Common.Collections.Synchronized;
using Chaos.Common.Utilities;
using Chaos.Containers;
using Chaos.Objects.Legend;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

/// <summary>
///     Manages save files for Aislings
/// </summary>
public sealed class UserSaveManager : BackgroundService, ISaveManager<Aisling>
{
    private readonly PeriodicTimer BackupTimer;
    private readonly ICloningService<Item> ItemCloningService;
    private readonly JsonSerializerOptions JsonSerializerOptions;
    private readonly SynchronizedHashSet<string> LockedFiles;
    private readonly ILogger<UserSaveManager> Logger;
    private readonly ITypeMapper Mapper;
    private readonly UserSaveManagerOptions Options;

    public UserSaveManager(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<UserSaveManagerOptions> options,
        ILogger<UserSaveManager> logger,
        ICloningService<Item> itemCloningService
    )
    {
        Options = options.Value;
        Mapper = mapper;
        Logger = logger;
        ItemCloningService = itemCloningService;
        JsonSerializerOptions = jsonSerializerOptions.Value;
        BackupTimer = new PeriodicTimer(TimeSpan.FromMinutes(Options.BackupIntervalMins));
        LockedFiles = new SynchronizedHashSet<string>(comparer: StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);

        if (!Directory.Exists(Options.BackupDirectory))
            Directory.CreateDirectory(Options.BackupDirectory);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var dop = Math.Max(1, Environment.ProcessorCount / 4);
        var options = new ParallelOptions { MaxDegreeOfParallelism = dop };

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await BackupTimer.WaitForNextTickAsync(stoppingToken);
                var start = Stopwatch.GetTimestamp();

                Logger.LogTrace("Performing backup");

                await Parallel.ForEachAsync(
                    Directory.EnumerateDirectories(Options.Directory),
                    options,
                    TakeBackupAsync);

                Parallel.ForEach(Directory.EnumerateDirectories(Options.BackupDirectory), options, HandleBackupRetention);

                Logger.LogDebug("Backup completed, took {@Elapsed}", Stopwatch.GetElapsedTime(start));
            } catch (OperationCanceledException)
            {
                //ignore
                return;
            }
    }

    private void HandleBackupRetention(string directory)
    {
        var directoryInfo = new DirectoryInfo(directory);

        if (!directoryInfo.Exists)
        {
            Logger.LogError("Failed to handle backup retention for \"{Directory}\" because it doesn't exist", directory);

            return;
        }

        var deleteTime = DateTime.UtcNow.AddDays(-Options.BackupRetentionDays);

        foreach (var fileInfo in directoryInfo.EnumerateFiles())
            if (fileInfo.CreationTimeUtc < deleteTime)
                try
                {
                    Logger.LogTrace("Deleting backup \"{Backup}\"", fileInfo.FullName);
                    fileInfo.Delete();
                } catch
                {
                    //ignored
                }
    }

    private Task InnerSaveAsync(string directory, Aisling aisling) => Task.WhenAll(
        MapAndSerializeAsync(
            directory,
            "aisling.json",
            Mapper.Map<AislingSchema>,
            aisling),
        MapAndSerializeAsync(
            directory,
            "bank.json",
            Mapper.Map<BankSchema>,
            aisling.Bank),
        MapAndSerializeAsync(
            directory,
            "trackers.json",
            Mapper.Map<TrackersSchema>,
            aisling.Trackers),
        MapAndSerializeAsync(
            directory,
            "legend.json",
            Mapper.MapMany<LegendMarkSchema>,
            aisling.Legend),
        MapAndSerializeAsync(
            directory,
            "inventory.json",
            Mapper.MapMany<ItemSchema>,
            aisling.Inventory),
        MapAndSerializeAsync(
            directory,
            "skills.json",
            Mapper.MapMany<SkillSchema>,
            aisling.SkillBook),
        MapAndSerializeAsync(
            directory,
            "spells.json",
            Mapper.MapMany<SpellSchema>,
            aisling.SpellBook),
        MapAndSerializeAsync(
            directory,
            "equipment.json",
            Mapper.MapMany<ItemSchema>,
            aisling.Equipment),
        MapAndSerializeAsync(
            directory,
            "effects.json",
            Mapper.MapMany<IEffect, EffectSchema>,
            aisling.Effects));

    public async Task<Aisling> LoadAsync(string name)
    {
        Logger.LogTrace("Loading aisling {Name}", name);

        var directory = Path.Combine(Options.Directory, name.ToLower());

        var aislingSchema = await JsonSerializerEx.DeserializeAsync<AislingSchema>(
            Path.Combine(directory, "aisling.json"),
            JsonSerializerOptions);

        var bankSchema = await JsonSerializerEx.DeserializeAsync<BankSchema>(Path.Combine(directory, "bank.json"), JsonSerializerOptions);

        var effectsSchema = await JsonSerializerEx.DeserializeAsync<IEnumerable<EffectSchema>>(
            Path.Combine(directory, "effects.json"),
            JsonSerializerOptions);

        var equipmentSchema = await JsonSerializerEx.DeserializeAsync<IEnumerable<ItemSchema>>(
            Path.Combine(directory, "equipment.json"),
            JsonSerializerOptions);

        var inventorySchema = await JsonSerializerEx.DeserializeAsync<IEnumerable<ItemSchema>>(
            Path.Combine(directory, "inventory.json"),
            JsonSerializerOptions);

        var skillsSchemas = await JsonSerializerEx.DeserializeAsync<IEnumerable<SkillSchema>>(
            Path.Combine(directory, "skills.json"),
            JsonSerializerOptions);

        var spellsSchemas = await JsonSerializerEx.DeserializeAsync<IEnumerable<SpellSchema>>(
            Path.Combine(directory, "spells.json"),
            JsonSerializerOptions);

        var legendSchema = await JsonSerializerEx.DeserializeAsync<IEnumerable<LegendMarkSchema>>(
            Path.Combine(directory, "legend.json"),
            JsonSerializerOptions);

        var trackersSchema = await JsonSerializerEx.DeserializeAsync<TrackersSchema>(
            Path.Combine(directory, "trackers.json"),
            JsonSerializerOptions);

        var aisling = Mapper.Map<Aisling>(aislingSchema!);
        var bank = Mapper.Map<Bank>(bankSchema!);
        var effects = new EffectsBar(aisling, Mapper.MapMany<EffectSchema, IEffect>(effectsSchema!));
        var equipment = new Equipment(Mapper.MapMany<Item>(equipmentSchema!));
        var inventory = new Inventory(ItemCloningService, Mapper.MapMany<Item>(inventorySchema!));
        var skillBook = new SkillBook(Mapper.MapMany<Skill>(skillsSchemas!));
        var spellBook = new SpellBook(Mapper.MapMany<Spell>(spellsSchemas!));
        var legend = new Legend(Mapper.MapMany<LegendMark>(legendSchema!));
        var trackers = Mapper.Map<Trackers>(trackersSchema!);

        aisling.Initialize(
            name,
            bank,
            equipment,
            inventory,
            skillBook,
            spellBook,
            legend,
            effects,
            trackers);

        Logger.LogDebug("Loaded {@Player}", aisling);

        return aisling;
    }

    private Task MapAndSerializeAsync<T, TSchema>(
        string directory,
        string fileName,
        Func<T, TSchema> mapper,
        T obj
    )
    {
        var schema = mapper(obj)!;
        var path = Path.Combine(directory, fileName);

        return JsonSerializerEx.SerializeAsync(path, schema, JsonSerializerOptions);
    }

    private async Task SafeExecuteDirectoryActionAsync(string directory, Func<Task> action)
    {
        while (!LockedFiles.Add(directory))
            await Task.Delay(1);

        try
        {
            await action();
        } catch (Exception e)
        {
            Logger.LogError(e, "Failed to execute directory action for \"{Directory}\"", directory);
        } finally
        {
            LockedFiles.Remove(directory);
        }
    }

    public async Task SaveAsync(Aisling obj)
    {
        Logger.LogTrace("Saving {@Player}", obj);
        var start = Stopwatch.GetTimestamp();

        try
        {
            var directory = Path.Combine(Options.Directory, obj.Name.ToLower());

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await SafeExecuteDirectoryActionAsync(directory, () => InnerSaveAsync(directory, obj));

            Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);
            Logger.LogDebug("Saved {@Player}, took {@Elapsed}", obj, Stopwatch.GetElapsedTime(start));

            /*
            var aislingSchema = Mapper.Map<AislingSchema>(obj);
            var bankSchema = Mapper.Map<BankSchema>(obj.Bank);
            var equipmentSchema = Mapper.MapMany<ItemSchema>(obj.Equipment).ToList();
            var inventorySchema = Mapper.MapMany<ItemSchema>(obj.Inventory).ToList();
            var effectsSchemas = Mapper.MapMany<IEffect, EffectSchema>(obj.Effects).ToList();
            var skillsSchemas = Mapper.MapMany<SkillSchema>(obj.SkillBook).ToList();
            var spellsSchemas = Mapper.MapMany<SpellSchema>(obj.SpellBook).ToList();
            var legendSchema = Mapper.MapMany<LegendMarkSchema>(obj.Legend).ToList();
            var trackersSchema = Mapper.Map<TrackersSchema>(obj.Trackers);

            var directory = Path.Combine(Options.Directory, obj.Name.ToLower());

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await SafeExecuteDirectoryActionAsync(
                directory,
                async () =>
                {
                    await Task.WhenAll(
                        JsonSerializerEx.SerializeAsync(Path.Combine(directory, "aisling.json"), aislingSchema, JsonSerializerOptions),
                        JsonSerializerEx.SerializeAsync(Path.Combine(directory, "bank.json"), bankSchema, JsonSerializerOptions),
                        JsonSerializerEx.SerializeAsync(Path.Combine(directory, "trackers.json"), trackersSchema, JsonSerializerOptions),
                        JsonSerializerEx.SerializeAsync(Path.Combine(directory, "legend.json"), legendSchema, JsonSerializerOptions),
                        JsonSerializerEx.SerializeAsync(Path.Combine(directory, "inventory.json"), inventorySchema, JsonSerializerOptions),
                        JsonSerializerEx.SerializeAsync(Path.Combine(directory, "skills.json"), skillsSchemas, JsonSerializerOptions),
                        JsonSerializerEx.SerializeAsync(Path.Combine(directory, "spells.json"), spellsSchemas, JsonSerializerOptions),
                        JsonSerializerEx.SerializeAsync(Path.Combine(directory, "equipment.json"), equipmentSchema, JsonSerializerOptions),
                        JsonSerializerEx.SerializeAsync(Path.Combine(directory, "effects.json"), effectsSchemas, JsonSerializerOptions));

                    Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);
                    Logger.LogDebug("Saved {@Player} in {Elapsed}", obj, Stopwatch.GetElapsedTime(start));
                });
                */
        } catch (Exception e)
        {
            Logger.LogError(
                e,
                "Failed to save aisling {@Player} in {Elapsed}",
                obj,
                Stopwatch.GetElapsedTime(start));
        }
    }

    private async ValueTask TakeBackupAsync(string saveDirectory, CancellationToken token)
    {
        try
        {
            var directoryInfo = new DirectoryInfo(saveDirectory);

            if (!directoryInfo.Exists)
            {
                Logger.LogError("Failed to take backup for path \"{SaveDir}\" because it doesn't exist", saveDirectory);

                return;
            }

            //don't take backups of things that haven't been modified
            if (directoryInfo.LastWriteTimeUtc < DateTime.UtcNow.AddMinutes(-Options.BackupIntervalMins))
                return;

            var directoryName = Path.GetFileName(saveDirectory);
            var backupDirectory = Path.Combine(Options.BackupDirectory, directoryName);

            if (!Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);

            var backupPath = Path.Combine(backupDirectory, $"{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.zip");

            if (token.IsCancellationRequested)
                return;

            await SafeExecuteDirectoryActionAsync(
                saveDirectory,
                () =>
                {
                    Logger.LogTrace("Backing up directory \"{SaveDir}\"", saveDirectory);
                    ZipFile.CreateFromDirectory(saveDirectory, backupPath);

                    return Task.CompletedTask;
                });
        } catch (Exception e)
        {
            Logger.LogError(e, "Failed to take backup for path \"{SaveDir}\"", saveDirectory);
        }
    }
}