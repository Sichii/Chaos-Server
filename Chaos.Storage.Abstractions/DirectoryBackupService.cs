#region
using System.IO.Compression;
using Chaos.IO.FileSystem;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Storage.Abstractions;

/// <inheritdoc cref="IDirectoryBackupService" />
public class DirectoryBackupService<TOptions> : BackgroundService, IDirectoryBackupService where TOptions: class, IDirectoryBackupOptions
{
    /// <summary>
    ///     The logger used to log events
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    ///     The options used to configure the store
    /// </summary>
    protected TOptions Options { get; }

    /// <summary>
    ///     Creates a new instance of <see cref="DirectoryBackupService{TOptions}" />
    /// </summary>
    public DirectoryBackupService(IOptions<TOptions> options, ILogger<DirectoryBackupService<TOptions>> logger)
    {
        Options = options.Value;
        Logger = logger;
    }

    /// <inheritdoc />
    public virtual ValueTask HandleBackupRetentionAsync(string directory, CancellationToken token)
    {
        var directoryInfo = new DirectoryInfo(directory);

        if (!directoryInfo.Exists)
        {
            Logger.WithTopics(Topics.Entities.Backup, Topics.Actions.Save)
                  .LogError("Failed to handle backup retention for path {@Directory} because it doesn't exist", directory);

            return default;
        }

        var deleteTime = DateTime.UtcNow.AddDays(-Options.BackupRetentionDays);

        foreach (var fileInfo in directoryInfo.EnumerateFiles())
            if (fileInfo.CreationTimeUtc < deleteTime)
                try
                {
                    Logger.WithTopics(Topics.Entities.Backup, Topics.Actions.Delete)
                          .LogTrace("Deleting backup {@Backup}", fileInfo.FullName);

                    var metricsLogger = Logger.WithTopics(Topics.Entities.Backup, Topics.Actions.Delete)
                                              .WithMetrics();

                    fileInfo.Delete();

                    metricsLogger.LogTrace("Backup deleted {@Backup}", fileInfo.FullName);
                } catch
                {
                    //ignored, not even worth logging
                }

        return default;
    }

    /// <inheritdoc />
    public virtual ValueTask TakeBackupAsync(string saveDirectory, CancellationToken token)
    {
        try
        {
            var directoryInfo = new DirectoryInfo(saveDirectory);

            if (!directoryInfo.Exists)
            {
                Logger.WithTopics(Topics.Entities.Backup, Topics.Actions.Save)
                      .LogError("Failed to take backup for path {@SaveDir} because it doesn't exist", saveDirectory);

                return default;
            }

            //don't take backups of things that haven't been modified
            if (directoryInfo.LastWriteTimeUtc < DateTime.UtcNow.AddMinutes(-Options.BackupIntervalMins))
                return default;

            var directoryName = Path.GetFileName(saveDirectory);
            var backupDirectory = Path.Combine(Options.BackupDirectory, directoryName);

            if (!Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);

            var backupPath = Path.Combine(backupDirectory, $"{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.zip");

            if (token.IsCancellationRequested)
                return default;

            saveDirectory.SafeExecute(saveDir =>
            {
                Logger.WithTopics(Topics.Entities.Backup, Topics.Actions.Save)
                      .LogTrace("Backing up directory {@SaveDir}", saveDirectory);

                var metricsLogger = Logger.WithTopics(Topics.Entities.Backup, Topics.Actions.Save)
                                          .WithMetrics();

                ZipFile.CreateFromDirectory(saveDir, backupPath);

                metricsLogger.LogTrace("Backup completed for {@SaveDir}", saveDirectory);
            });
        } catch (Exception e)
        {
            Logger.WithTopics(Topics.Entities.Backup, Topics.Actions.Save)
                  .LogError(e, "Failed to take backup for path {@SaveDir}", saveDirectory);
        }

        return default;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var dop = Math.Max(1, Environment.ProcessorCount / 4);

        var pOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = dop
        };
        var backupTimer = new PeriodicTimer(TimeSpan.FromMinutes(Options.BackupIntervalMins));

        if (!Directory.Exists(Options.BackupDirectory))
            Directory.CreateDirectory(Options.BackupDirectory);

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await backupTimer.WaitForNextTickAsync(stoppingToken)
                                 .ConfigureAwait(false);

                Logger.WithTopics(Topics.Entities.Backup, Topics.Actions.Save)
                      .LogDebug("Performing backup");

                var metricsLogger = Logger.WithTopics(Topics.Entities.Backup, Topics.Actions.Save)
                                          .WithMetrics();

                await Parallel.ForEachAsync(Directory.EnumerateDirectories(Options.Directory), pOptions, TakeBackupAsync);

                await Parallel.ForEachAsync(Directory.EnumerateDirectories(Options.BackupDirectory), pOptions, HandleBackupRetentionAsync);

                metricsLogger.WithTopics(Topics.Entities.Backup, Topics.Actions.Save)
                             .LogDebug("Backup completed");
            } catch (OperationCanceledException)
            {
                //ignore
                return;
            } catch (Exception e)
            {
                Logger.WithTopics(Topics.Entities.Backup, Topics.Actions.Save)
                      .LogError(e, "Exception occurred while performing backup");
            }
    }
}