using Chaos.Collections;
using Chaos.IO.FileSystem;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Schemas.Guilds;
using Chaos.Services.Storage.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

/// <summary>
///     A store for <see cref="Guild" />s.
/// </summary>
public class GuildStore : PeriodicSaveStoreBase<Guild, GuildStoreOptions>
{
    /// <inheritdoc />
    public GuildStore(IEntityRepository entityRepository, IOptions<GuildStoreOptions> options, ILogger<GuildStore> logger)
        : base(entityRepository, options, logger) { }

    private void InnerSave(string directory, Guild guild)
    {
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

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

        EntityRepository.SaveAndMap<Guild, GuildSchema>(guild, guildPath);
        EntityRepository.SaveAndMap<GuildRank, GuildRankSchema>(tier0, tier0Path);
        EntityRepository.SaveAndMap<GuildRank, GuildRankSchema>(tier1, tier1Path);
        EntityRepository.SaveAndMap<GuildRank, GuildRankSchema>(tier2, tier2Path);
        EntityRepository.SaveAndMap<GuildRank, GuildRankSchema>(tier3, tier3Path);
    }

    private Task InnerSaveAsync(string directory, Guild guild)
    {
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

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

    /// <inheritdoc />
    protected override Guild LoadFromFile(string dir, string key)
    {
        Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Load)
              .LogDebug("Loading new {@TypeName} entry with key {@Key}", nameof(Guild), key);

        var metricsLogger = Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Load)
                                  .WithMetrics();

        if (!Directory.Exists(dir))
            throw new InvalidOperationException($"Directory {dir} does not exist");

        var guildPath = Path.Combine(dir, "guild.json");
        var tier0Path = Path.Combine(dir, "tier0.json");
        var tier1Path = Path.Combine(dir, "tier1.json");
        var tier2Path = Path.Combine(dir, "tier2.json");
        var tier3Path = Path.Combine(dir, "tier3.json");

        var guild = EntityRepository.LoadAndMap<Guild, GuildSchema>(guildPath);
        var tier0 = EntityRepository.LoadAndMap<GuildRank, GuildRankSchema>(tier0Path);
        var tier1 = EntityRepository.LoadAndMap<GuildRank, GuildRankSchema>(tier1Path);
        var tier2 = EntityRepository.LoadAndMap<GuildRank, GuildRankSchema>(tier2Path);
        var tier3 = EntityRepository.LoadAndMap<GuildRank, GuildRankSchema>(tier3Path);

        guild.Initialize(
            new[]
            {
                tier0,
                tier1,
                tier2,
                tier3
            });

        metricsLogger.LogDebug("Loaded new {@TypeName} entry with {@Key}", nameof(Guild), key);

        return guild;
    }

    /// <inheritdoc />
    public override void Save(Guild obj)
    {
        Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Save)
              .WithProperty(obj)
              .LogDebug("Saving {@TypeName} entry with key {@Key}", nameof(Guild), obj.Name);

        var metricsLogger = Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Save)
                                  .WithMetrics()
                                  .WithProperty(obj);

        try
        {
            var directory = Path.Combine(Options.Directory, obj.Name);

            directory.SafeExecute(
                dir =>
                {
                    InnerSave(dir, obj);
                    Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);
                });

            metricsLogger.LogDebug("Saved obj {@GuildName}", obj.Name);
        } catch (Exception e)
        {
            metricsLogger.LogError(e, "Failed to save obj {@GuildName}", obj.Name);
        }
    }

    /// <inheritdoc />
    public override async Task SaveAsync(Guild obj)
    {
        Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Save)
              .WithProperty(obj)
              .LogTrace("Saving {@TypeName} entry with key {@Key}", nameof(Guild), obj.Name);

        var metricsLogger = Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Save)
                                  .WithMetrics()
                                  .WithProperty(obj);

        try
        {
            var directory = Path.Combine(Options.Directory, obj.Name);

            await directory.SafeExecuteAsync(
                async dir =>
                {
                    await InnerSaveAsync(dir, obj);
                    Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);
                });

            metricsLogger.LogTrace("Saved obj {@GuildName}", obj.Name);
        } catch (Exception e)
        {
            metricsLogger.LogError(e, "Failed to save obj {@GuildName}", obj.Name);
        }
    }
}