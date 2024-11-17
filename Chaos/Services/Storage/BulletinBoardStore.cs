#region
using Chaos.Collections;
using Chaos.IO.FileSystem;
using Chaos.Models.Templates;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Schemas.Boards;
using Chaos.Services.Storage.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Services.Storage;

public sealed class BulletinBoardStore : PeriodicSaveStoreBase<BulletinBoard, BulletinBoardStoreOptions>
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public BulletinBoardStore(
        IEntityRepository entityRepository,
        IOptions<BulletinBoardStoreOptions> options,
        ILogger<BulletinBoardStore> logger,
        ISimpleCache simpleCache)
        : base(entityRepository, options, logger)
        => SimpleCache = simpleCache;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var saveTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        var deltaTime = new DeltaTime();
        var saveInterval = new IntervalTimer(TimeSpan.FromMinutes(Options.SaveIntervalMins), false);

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await saveTimer.WaitForNextTickAsync(stoppingToken);
                var delta = deltaTime.GetDelta;

                foreach (var board in Cache.Values)
                    board.Script.Update(delta);

                saveInterval.Update(delta);

                if (saveInterval.IntervalElapsed)
                {
                    var metricsLogger = Logger.WithTopics(
                                                  [
                                                      Topics.Entities.BulletinBoard,
                                                      Topics.Actions.Save
                                                  ])
                                              .WithMetrics();

                    Logger.LogDebug("Performing save");
                    var boards = Cache.Values.ToList();

                    await Task.WhenAll(boards.Select(SaveAsync));

                    metricsLogger.LogDebug("Save completed");
                }
            } catch (OperationCanceledException)
            {
                //ignore
                break;
            } catch (Exception e)
            {
                Logger.WithTopics(
                          [
                              Topics.Entities.BulletinBoard,
                              Topics.Actions.Save
                          ])
                      .LogError(e, "Exception while performing save");
            }

        Logger.WithTopics(
                  [
                      Topics.Entities.BulletinBoard,
                      Topics.Actions.Save
                  ])
              .LogInformation("Performing final save before shutdown");

        var metricsLoggerA = Logger.WithTopics(
                                       [
                                           Topics.Entities.BulletinBoard,
                                           Topics.Actions.Save
                                       ])
                                   .WithMetrics();

        var guildsToSave = Cache.Values.ToList();
        await Task.WhenAll(guildsToSave.Select(SaveAsync));

        metricsLoggerA.LogInformation("Final save completed");
    }

    /// <inheritdoc />
    public override bool Exists(string key)
    {
        try
        {
            SimpleCache.Get<BulletinBoardTemplate>(key);
        } catch
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    protected override BulletinBoard LoadFromFile(string dir, string key)
    {
        Logger.WithTopics(
                  [
                      Topics.Entities.BulletinBoard,
                      Topics.Actions.Load
                  ])
              .LogDebug("Loading new {@TypeName} entry with key {@Key}", nameof(BulletinBoard), key);

        var metricsLogger = Logger.WithTopics(
                                      [
                                          Topics.Entities.BulletinBoard,
                                          Topics.Actions.Load
                                      ])
                                  .WithMetrics();

        if (!Exists(key))
            throw new DirectoryNotFoundException($"Board configuration with key \"{key}\" does not exist");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var path = Path.Combine(dir, "board.json");

        if (!File.Exists(path))
        {
            var initial = new BulletinBoardSchema
            {
                TemplateKey = key
            };

            EntityRepository.Save(initial, path);
        }

        var ret = EntityRepository.LoadAndMap<BulletinBoard, BulletinBoardSchema>(path);
        ret.Script.Update(TimeSpan.Zero);

        metricsLogger.LogDebug("Loaded new {@TypeName} entry with {@Key}", nameof(BulletinBoard), key);

        return ret;
    }

    /// <inheritdoc />
    public override void Save(BulletinBoard obj)
    {
        Logger.WithTopics(
                  [
                      Topics.Entities.BulletinBoard,
                      Topics.Actions.Save
                  ])
              .WithProperty(obj)
              .LogDebug("Saving {@TypeName} entry with key {@Key}", nameof(BulletinBoard), obj.Key);

        var metricsLogger = Logger.WithTopics(
                                      [
                                          Topics.Entities.BulletinBoard,
                                          Topics.Actions.Save
                                      ])
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

                    var path = Path.Combine(dir, "board.json");

                    EntityRepository.SaveAndMap<BulletinBoard, BulletinBoardSchema>(obj, path);
                    Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);
                });

            metricsLogger.LogDebug("Saved {@TypeName} entry with key {@Key}", nameof(BulletinBoard), obj.Key);
        } catch (Exception e)
        {
            metricsLogger.LogError(
                e,
                "Failed to save {@TypeName} entry with key {@Key}",
                nameof(BulletinBoard),
                obj.Key);
        }
    }

    /// <inheritdoc />
    public override async Task SaveAsync(BulletinBoard obj)
    {
        Logger.WithTopics(
                  [
                      Topics.Entities.BulletinBoard,
                      Topics.Actions.Save
                  ])
              .WithProperty(obj)
              .LogTrace("Saving {@TypeName} entry with key {@Key}", nameof(BulletinBoard), obj.Key);

        var metricsLogger = Logger.WithTopics(
                                      [
                                          Topics.Entities.BulletinBoard,
                                          Topics.Actions.Save
                                      ])
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

                    var path = Path.Combine(dir, "board.json");

                    await EntityRepository.SaveAndMapAsync<BulletinBoard, BulletinBoardSchema>(obj, path);
                    Directory.SetLastWriteTimeUtc(directory, DateTime.UtcNow);
                });

            metricsLogger.LogTrace("Saved {@TypeName} entry with key {@Key}", nameof(BulletinBoard), obj.Key);
        } catch (Exception e)
        {
            metricsLogger.LogError(
                e,
                "Failed to save {@TypeName} entry with key {@Key}",
                nameof(BulletinBoard),
                obj.Key);
        }
    }
}