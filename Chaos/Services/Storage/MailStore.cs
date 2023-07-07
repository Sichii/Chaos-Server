using System.Diagnostics;
using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.IO.FileSystem;
using Chaos.Schemas.Boards;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public class MailStore : BackgroundService, IStore<MailBox>
{
    private readonly ConcurrentDictionary<string, MailBox> Cache;
    private readonly IEntityRepository EntityRepository;
    private readonly ILogger<MailStore> Logger;
    private readonly MailStoreOptions Options;

    public MailStore(IEntityRepository entityRepository, IOptions<MailStoreOptions> options, ILogger<MailStore> logger)
    {
        Cache = new ConcurrentDictionary<string, MailBox>(StringComparer.OrdinalIgnoreCase);
        EntityRepository = entityRepository;
        Options = options.Value;
        Logger = logger;
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
    public MailBox Load(string key)
    {
        var directory = Path.Combine(Options.Directory, key);

        MailBox InnerLoad(string dir) => Cache.GetOrAdd(key, InnerLoadMailBox, (dir, EntityRepository, Logger));

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
    public void Save(MailBox obj)
    {
        if (string.IsNullOrEmpty(obj.Key))
            throw new InvalidOperationException("Mailbox owner name cannot be null or empty");

        if (!Cache.TryAdd(obj.Key, obj))
            throw new InvalidOperationException($"Mailbox for {obj.Key} is already being saved");

        AsyncHelpers.RunSync(() => SaveMailBoxAsync(obj));

        Logger.LogDebug("Saved new mailbox for {@AislingName}", obj.Key);
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
                var mailBoxes = Cache.Values.ToList();

                await Task.WhenAll(mailBoxes.Select(SaveMailBoxAsync));

                Logger.LogDebug("Save completed, took {@Elapsed}", Stopwatch.GetElapsedTime(start));
            } catch (OperationCanceledException)
            {
                //ignore
                break;
            } catch (Exception e)
            {
                Logger.LogCritical(e, "Exception while performing save");
            }

        Logger.LogDebug("Performing final save before shutdown");

        var guildsToSave = Cache.Values.ToList();
        await Task.WhenAll(guildsToSave.Select(SaveMailBoxAsync));

        Logger.LogInformation("Final save completed");
    }

    private static MailBox InnerLoadMailBox(string key, (string, IEntityRepository, ILogger<MailStore>) args)
    {
        (var directory, var entityRepository, var logger) = args;

        logger.LogTrace("Loading mailbox for {@AislingName}", key);
        var start = Stopwatch.GetTimestamp();

        if (!Directory.Exists(directory))
            throw new InvalidOperationException($"No mailbox exists for the key \"{key}\" at the specified path \"{directory}\"");

        var path = Path.Combine(directory, "mailbox.json");

        var ret = entityRepository.LoadAndMap<MailBox, MailBoxSchema>(path);

        logger.LogDebug("Loaded mailbox for {@AislingName}, took {@Elapsed}", key, Stopwatch.GetElapsedTime(start));

        return ret;
    }

    private Task InnerSaveAsync(string directory, MailBox mailBox)
    {
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var mailboxPath = Path.Combine(directory, "mailbox.json");

        return EntityRepository.SaveAndMapAsync<MailBox, MailBoxSchema>(mailBox, mailboxPath);
    }

    protected virtual async Task SaveMailBoxAsync(MailBox mailBox)
    {
        if (string.IsNullOrEmpty(mailBox.Key))
            throw new InvalidOperationException("Mailbox owner name cannot be null or empty");

        Logger.WithProperty(mailBox)
              .LogTrace("Saving {@AislingName}'s mailbox", mailBox.Key);

        var start = Stopwatch.GetTimestamp();

        try
        {
            var directory = Path.Combine(Options.Directory, mailBox.Key);

            await directory.SafeExecuteAsync(
                async dir =>
                {
                    await InnerSaveAsync(dir, mailBox);
                    Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);
                });

            Logger.WithProperty(mailBox)
                  .LogDebug("Saved {@AislingName}'s mailbox, took {@Elapsed}", mailBox.Key, Stopwatch.GetElapsedTime(start));
        } catch (Exception e)
        {
            Logger.WithProperty(mailBox)
                  .LogCritical(
                      e,
                      "Failed to save {@AislingName}'s mailbox in {@Elapsed}",
                      mailBox.Key,
                      Stopwatch.GetElapsedTime(start));
        }
    }
}