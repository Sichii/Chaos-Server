using System.Diagnostics;
using Chaos.Collections;
using Chaos.Extensions;
using Chaos.IO.FileSystem;
using Chaos.Schemas.Guilds;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public class GuildStore : BackgroundService, IStore<Guild>
{
    private readonly ConcurrentDictionary<string, Guild> Cache;
    private readonly IEntityRepository EntityRepository;
    private readonly ILogger<GuildStore> Logger;
    private readonly GuildStoreOptions Options;

    public GuildStore(
        IEntityRepository entityRepository,
        IOptions<GuildStoreOptions> options,
        ILogger<GuildStore> logger
    )
    {
        Logger = logger;
        Options = options.Value;
        EntityRepository = entityRepository;
        Cache = new ConcurrentDictionary<string, Guild>(StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public bool Exists(string key)
    {
        var directory = Path.Combine(Options.Directory, key);

        bool InnerExists(string dir)
        {
            if (Directory.Exists(dir))
                return true;

            return Cache.ContainsKey(key);
        }

        return directory.SafeExecute(InnerExists);
    }

    /// <inheritdoc />
    public Guild Load(string key)
    {
        Logger.LogTrace("Loading guild {@GuildName}", key);

        var directory = Path.Combine(Options.Directory, key);

        Guild InnerLoad(string dir) => Cache.GetOrAdd(key, InnerLoadGuild, (dir, EntityRepository));

        return directory.SafeExecute(InnerLoad);
    }

    /// <inheritdoc />
    public bool Remove(string key)
    {
        var directory = Path.Combine(Options.Directory, key);

        bool InnerRemove(string dir)
        {
            if (!Directory.Exists(directory))
                return Cache.Remove(key, out _);

            Cache.Remove(key, out _);
            Directory.Delete(dir);

            return true;
        }

        return directory.SafeExecute(InnerRemove);
    }

    /// <inheritdoc />
    public void Save(Guild guild)
    {
        if (!Cache.TryAdd(guild.Name, guild))
            throw new InvalidOperationException($"The guild \"{guild.Name}\" is already being saved.");

        Logger.WithProperty(guild)
              .LogDebug("Added new guild {@GuildName} to guild store", guild.Name);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var saveTimer = new PeriodicTimer(TimeSpan.FromMinutes(Options.SaveIntervalMins));

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await saveTimer.WaitForNextTickAsync(stoppingToken);
                var start = Stopwatch.GetTimestamp();

                Logger.LogTrace("Performing save");
                var guilds = Cache.Values.ToList();

                await Task.WhenAll(guilds.Select(SaveGuildAsync));

                Logger.LogDebug("Save completed, took {@Elapsed}", Stopwatch.GetElapsedTime(start));
            } catch (OperationCanceledException)
            {
                //ignore
                return;
            } catch (Exception e)
            {
                Logger.LogCritical(e, "Exception while performing save");
            }

        Logger.LogDebug("Performing final save before shutdown");

        var guildsToSave = Cache.Values.ToList();
        await Task.WhenAll(guildsToSave.Select(SaveGuildAsync));

        Logger.LogInformation("Final save completed");
    }

    private static Guild InnerLoadGuild(string key, (string, IEntityRepository) args)
    {
        (var directory, var entityRepository) = args;

        if (!Directory.Exists(directory))
            throw new InvalidOperationException($"No guild data exists for the key \"{key}\" at the specified path \"{directory}\"");

        var guildPath = Path.Combine(directory, "guild.json");
        var tier0Path = Path.Combine(directory, "tier0.json");
        var tier1Path = Path.Combine(directory, "tier1.json");
        var tier2Path = Path.Combine(directory, "tier2.json");
        var tier3Path = Path.Combine(directory, "tier3.json");

        var guild = entityRepository.LoadAndMap<Guild, GuildSchema>(guildPath);
        var tier0 = entityRepository.LoadAndMap<GuildRank, GuildRankSchema>(tier0Path);
        var tier1 = entityRepository.LoadAndMap<GuildRank, GuildRankSchema>(tier1Path);
        var tier2 = entityRepository.LoadAndMap<GuildRank, GuildRankSchema>(tier2Path);
        var tier3 = entityRepository.LoadAndMap<GuildRank, GuildRankSchema>(tier3Path);

        guild.Initialize(new[] { tier0, tier1, tier2, tier3 });

        return guild;
    }

    private Task InnerSaveAsync(string directory, Guild guild)
    {
        var guildPath = Path.Combine(directory, "guild.json");
        var tier0Path = Path.Combine(directory, "tier0.json");
        var tier1Path = Path.Combine(directory, "tier1.json");
        var tier2Path = Path.Combine(directory, "tier2.json");
        var tier3Path = Path.Combine(directory, "tier3.json");

        if (!guild.TryGetRank(0, out var tier0))
            throw new InvalidOperationException("The guild does not have a tier 0 rank");

        if (!guild.TryGetRank(1, out var tier1))
            throw new InvalidOperationException("The guild does not have a tier 1 rank");

        if (!guild.TryGetRank(2, out var tier2))
            throw new InvalidOperationException("The guild does not have a tier 2 rank");

        if (!guild.TryGetRank(3, out var tier3))
            throw new InvalidOperationException("The guild does not have a tier 3 rank");

        return Task.WhenAll(
            EntityRepository.SaveAndMapAsync<Guild, GuildSchema>(guild, guildPath),
            EntityRepository.SaveAndMapAsync<GuildRank, GuildRankSchema>(tier0, tier0Path),
            EntityRepository.SaveAndMapAsync<GuildRank, GuildRankSchema>(tier1, tier1Path),
            EntityRepository.SaveAndMapAsync<GuildRank, GuildRankSchema>(tier2, tier2Path),
            EntityRepository.SaveAndMapAsync<GuildRank, GuildRankSchema>(tier3, tier3Path));
    }

    protected virtual async Task SaveGuildAsync(Guild guild)
    {
        Logger.WithProperty(guild)
              .LogTrace("Saving {@GuildName}", guild.Name);

        var start = Stopwatch.GetTimestamp();

        try
        {
            var directory = Path.Combine(Options.Directory, guild.Name);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await directory.SafeExecuteAsync(
                async dir =>
                {
                    await InnerSaveAsync(dir, guild);
                    Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);
                });

            Logger.WithProperty(guild)
                  .LogDebug("Saved guild {@GuildName}, took {@Elapsed}", guild.Name, Stopwatch.GetElapsedTime(start));
        } catch (Exception e)
        {
            Logger.WithProperty(guild)
                  .LogCritical(
                      e,
                      "Failed to save guild {@GuildName} in {@Elapsed}",
                      guild.Name,
                      Stopwatch.GetElapsedTime(start));
        }
    }
}