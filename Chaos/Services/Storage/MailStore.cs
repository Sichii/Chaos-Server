using Chaos.Collections;
using Chaos.IO.FileSystem;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Schemas.Boards;
using Chaos.Services.Storage.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public class MailStore : PeriodicSaveStoreBase<MailBox, MailStoreOptions>
{
    /// <inheritdoc />
    public MailStore(IEntityRepository entityRepository, IOptions<MailStoreOptions> options, ILogger<MailStore> logger)
        : base(entityRepository, options, logger) { }

    /// <inheritdoc />
    protected override MailBox LoadFromFile(string dir, string key)
    {
        Logger.WithTopics(Topics.Entities.MailBox, Topics.Actions.Load)
              .LogDebug("Loading new {@TypeName} entry with key {@Key}", nameof(MailBox), key);

        var metricsLogger = Logger.WithTopics(Topics.Entities.MailBox, Topics.Actions.Load)
                                  .WithMetrics();

        if (!Directory.Exists(dir))
            throw new InvalidOperationException($"Directory {dir} does not exist");

        var path = Path.Combine(dir, "mail.json");

        var ret = EntityRepository.LoadAndMap<MailBox, MailBoxSchema>(path);

        metricsLogger.LogDebug("Loaded new {@TypeName} entry with {@Key}", nameof(MailBox), key);

        return ret;
    }

    /// <inheritdoc />
    public override void Save(MailBox obj)
    {
        Logger.WithTopics(Topics.Entities.MailBox, Topics.Actions.Save)
              .WithProperty(obj)
              .LogDebug("Saving {@TypeName} entry with key {@Key}", nameof(MailBox), obj.Key);

        var metricsLogger = Logger.WithTopics(Topics.Entities.MailBox, Topics.Actions.Save)
                                  .WithMetrics()
                                  .WithProperty(obj);

        try
        {
            var directory = Path.Combine(Options.Directory, obj.Key);

            directory.SafeExecute(
                dir =>
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var path = Path.Combine(dir, "mail.json");

                    EntityRepository.SaveAndMap<MailBox, MailBoxSchema>(obj, path);
                    Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);
                });

            metricsLogger.LogDebug("Saved {@TypeName} entry with key {@Key}", nameof(MailBox), obj.Key);
        } catch (Exception e)
        {
            metricsLogger.LogError(
                e,
                "Failed to save {@TypeName} entry with key {@Key}",
                nameof(MailBox),
                obj.Key);
        }
    }

    /// <inheritdoc />
    public override async Task SaveAsync(MailBox obj)
    {
        Logger.WithTopics(Topics.Entities.MailBox, Topics.Actions.Save)
              .WithProperty(obj)
              .LogTrace("Saving {@TypeName} entry with key {@Key}", nameof(MailBox), obj.Key);

        var metricsLogger = Logger.WithTopics(Topics.Entities.MailBox, Topics.Actions.Save)
                                  .WithMetrics()
                                  .WithProperty(obj);

        try
        {
            var directory = Path.Combine(Options.Directory, obj.Key);

            await directory.SafeExecuteAsync(
                async dir =>
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var path = Path.Combine(dir, "mail.json");

                    await EntityRepository.SaveAndMapAsync<MailBox, MailBoxSchema>(obj, path);
                    Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);
                });

            metricsLogger.LogTrace("Saved {@TypeName} entry with key {@Key}", nameof(MailBox), obj.Key);
        } catch (Exception e)
        {
            metricsLogger.LogError(
                e,
                "Failed to save {@TypeName} entry with key {@Key}",
                nameof(MailBox),
                obj.Key);
        }
    }
}