using System.Diagnostics;
using Chaos.Collections;
using Chaos.Extensions;
using Chaos.IO.FileSystem;
using Chaos.Schemas.Boards;
using Chaos.Services.Storage.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;
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
        Logger.LogTrace("Loading new {@TypeName} entry with key {@Key}", nameof(MailBox), key);
        var start = Stopwatch.GetTimestamp();

        if (!Directory.Exists(dir))
            throw new InvalidOperationException($"Directory {dir} does not exist");

        var path = Path.Combine(dir, "mail.json");

        var ret = EntityRepository.LoadAndMap<MailBox, MailBoxSchema>(path);

        Logger.LogDebug(
            "Loaded new {@TypeName} entry with {@Key}, took {@Elapsed}",
            nameof(MailBox),
            key,
            Stopwatch.GetElapsedTime(start));

        return ret;
    }

    /// <inheritdoc />
    public override void Save(MailBox obj)
    {
        Logger.WithProperty(obj)
              .LogTrace("Saving {@TypeName} entry with key {@Key}", nameof(MailBox), obj.Key);

        var start = Stopwatch.GetTimestamp();

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

            Logger.WithProperty(obj)
                  .LogDebug(
                      "Saved {@TypeName} entry with key {@Key}, took {@Elapsed}",
                      nameof(MailBox),
                      obj.Key,
                      Stopwatch.GetElapsedTime(start));
        } catch (Exception e)
        {
            Logger.WithProperty(obj)
                  .LogCritical(
                      e,
                      "Failed to save {@TypeName} entry with key {@Key} in {@Elapsed}",
                      nameof(MailBox),
                      obj.Key,
                      Stopwatch.GetElapsedTime(start));
        }
    }

    /// <inheritdoc />
    public override async Task SaveAsync(MailBox obj)
    {
        Logger.WithProperty(obj)
              .LogTrace("Saving {@TypeName} entry with key {@Key}", nameof(MailBox), obj.Key);

        var start = Stopwatch.GetTimestamp();

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

            Logger.WithProperty(obj)
                  .LogDebug(
                      "Saved {@TypeName} entry with key {@Key}, took {@Elapsed}",
                      nameof(MailBox),
                      obj.Key,
                      Stopwatch.GetElapsedTime(start));
        } catch (Exception e)
        {
            Logger.WithProperty(obj)
                  .LogCritical(
                      e,
                      "Failed to save {@TypeName} entry with key {@Key} in {@Elapsed}",
                      nameof(MailBox),
                      obj.Key,
                      Stopwatch.GetElapsedTime(start));
        }
    }
}