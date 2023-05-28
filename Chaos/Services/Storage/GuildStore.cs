using System.Diagnostics;
using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Schemas.Guilds;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public class GuildStore : BackedUpFileStoreBase<Guild, GuildStoreOptions>, IStore<Guild>
{
    private readonly ConcurrentDictionary<string, Guild> Cache;
    private readonly IEntityRepository EntityRepository;

    /// <inheritdoc />
    public GuildStore(
        IEntityRepository entityRepository,
        IOptions<GuildStoreOptions> options,
        ILogger<GuildStore> logger
    )
        : base(options, logger)
    {
        EntityRepository = entityRepository;
        Cache = new ConcurrentDictionary<string, Guild>(StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.WhenAll(
            base.ExecuteAsync(stoppingToken),
            PeriodicSaveAsync(stoppingToken));

        Logger.LogDebug("Performing final save before shutdown");

        await Task.WhenAll(Cache.Values.Select(SaveGuildAsync));

        Logger.LogInformation("Final save completed");
    }

    /// <inheritdoc />
    public bool Exists(string key)
    {
        var directory = Path.Combine(Options.Directory, key);

        if (Directory.Exists(directory))
            return true;

        return Cache.ContainsKey(key);
    }

    private static Guild InnerLoadGuild(string key, (string directory, IEntityRepository entityRepository) args)
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
            EntityRepository.SaveAsync<Guild, GuildSchema>(guild, guildPath),
            EntityRepository.SaveAsync<GuildRank, GuildRankSchema>(tier0, tier0Path),
            EntityRepository.SaveAsync<GuildRank, GuildRankSchema>(tier1, tier1Path),
            EntityRepository.SaveAsync<GuildRank, GuildRankSchema>(tier2, tier2Path),
            EntityRepository.SaveAsync<GuildRank, GuildRankSchema>(tier3, tier3Path));
    }

    /// <inheritdoc />
    public Guild Load(string key)
    {
        Logger.LogTrace("Loading guild {@GuildName}", key);

        var directory = Path.Combine(Options.Directory, key);

        return Cache.GetOrAdd(key, InnerLoadGuild, (path: directory, EntityRepository));
    }

    private async Task PeriodicSaveAsync(CancellationToken stoppingToken)
    {
        var saveTimer = new PeriodicTimer(TimeSpan.FromMinutes(Options.SaveIntervalMins));

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await saveTimer.WaitForNextTickAsync(stoppingToken);
                var start = Stopwatch.GetTimestamp();

                Logger.LogTrace("Performing save");

                await Task.WhenAll(Cache.Values.Select(SaveGuildAsync));

                Logger.LogDebug("Save completed, took {@Elapsed}", Stopwatch.GetElapsedTime(start));
            } catch (OperationCanceledException)
            {
                //ignore
                return;
            } catch (Exception e)
            {
                Logger.LogCritical(e, "Exception while performing save");
            }
    }

    /// <inheritdoc />
    public void Save(Guild guild)
    {
        if (!Cache.TryAdd(guild.Name, guild))
            throw new InvalidOperationException($"The guild \"{guild.Name}\" is already being saved.");

        Logger.WithProperty(guild)
              .LogInformation("Added new guild {@GuildName} to guild store", guild.Name);
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

            await SafeExecuteDirectoryActionAsync(directory, () => InnerSaveAsync(directory, guild));

            Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);

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