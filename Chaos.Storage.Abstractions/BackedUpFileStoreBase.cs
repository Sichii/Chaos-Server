using System.Diagnostics;
using System.IO.Compression;
using Chaos.Common.Collections.Synchronized;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage.Abstractions;

/// <inheritdoc cref="IBackedUpFileStore" />
public abstract class BackedUpFileStoreBase<TOptions> : BackgroundService, IBackedUpFileStore
    where TOptions: class, IBackedUpFileStoreOptions
{
    /// <summary>
    ///     The set of directory or file names that are currently locked (they are being used by something)
    /// </summary>
    protected SynchronizedHashSet<string> LockedFiles { get; }
    /// <summary>
    ///     The logger used to log events
    /// </summary>
    protected ILogger Logger { get; }
    /// <summary>
    ///     The options used to configure the store
    /// </summary>
    protected TOptions Options { get; }

    /// <summary>
    ///     Creates a new instance of <see cref="BackedUpFileStoreBase{TOptions}" />
    /// </summary>
    protected BackedUpFileStoreBase(
        IOptions<TOptions> options,
        ILogger logger
    )
    {
        Options = options.Value;
        Logger = logger;
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
        var backupTimer = new PeriodicTimer(TimeSpan.FromMinutes(Options.BackupIntervalMins));

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await backupTimer.WaitForNextTickAsync(stoppingToken);
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
            } catch (Exception e)
            {
                Logger.LogCritical(e, "Exception occurred while performing backup");
            }
    }

    /// <inheritdoc />
    public virtual void HandleBackupRetention(string directory)
    {
        var directoryInfo = new DirectoryInfo(directory);

        if (!directoryInfo.Exists)
        {
            Logger.LogError("Failed to handle backup retention for path {@Directory} because it doesn't exist", directory);

            return;
        }

        var deleteTime = DateTime.UtcNow.AddDays(-Options.BackupRetentionDays);

        foreach (var fileInfo in directoryInfo.EnumerateFiles())
            if (fileInfo.CreationTimeUtc < deleteTime)
                try
                {
                    Logger.LogTrace("Deleting backup {@Backup}", fileInfo.FullName);
                    fileInfo.Delete();
                } catch
                {
                    //ignored
                }
    }

    /// <summary>
    ///     Executes the specified action on the specified directory, ensuring that no other actions are being performed on the
    ///     directory
    /// </summary>
    /// <param name="directory">The directory to lock</param>
    /// <param name="action">The action to perform</param>
    protected virtual void SafeExecuteDirectoryAction(string directory, Action action)
    {
        SpinWait.SpinUntil(() => LockedFiles.Add(directory));

        try
        {
            action();
        } finally
        {
            LockedFiles.Remove(directory);
        }
    }

    /// <summary>
    ///     Executes the specified action on the specified directory, ensuring that no other actions are being performed on the
    ///     directory
    /// </summary>
    /// <param name="directory">The directory to lock</param>
    /// <param name="action">The action to perform</param>
    protected virtual TResult SafeExecuteDirectoryAction<TResult>(string directory, Func<TResult> action)
    {
        SpinWait.SpinUntil(() => LockedFiles.Add(directory));

        try
        {
            return action();
        } finally
        {
            LockedFiles.Remove(directory);
        }
    }

    /// <summary>
    ///     Executes the specified action on the specified directory, ensuring that no other actions are being performed on the
    ///     directory
    /// </summary>
    /// <param name="directory">The directory to lock</param>
    /// <param name="action">The action to perform</param>
    protected virtual async Task SafeExecuteDirectoryActionAsync(string directory, Func<Task> action)
    {
        while (!LockedFiles.Add(directory))
            await Task.Delay(1);

        try
        {
            await action();
        } finally
        {
            LockedFiles.Remove(directory);
        }
    }

    /// <summary>
    ///     Executes the specified action on the specified directory, ensuring that no other actions are being performed on the
    ///     directory
    /// </summary>
    /// <param name="directory">The directory to lock</param>
    /// <param name="action">The action to perform</param>
    protected virtual async Task<T> SafeExecuteDirectoryActionAsync<T>(string directory, Func<Task<T>> action)
    {
        while (!LockedFiles.Add(directory))
            await Task.Yield();

        try
        {
            return await action();
        } finally
        {
            LockedFiles.Remove(directory);
        }
    }

    /// <inheritdoc />
    public virtual async ValueTask TakeBackupAsync(string saveDirectory, CancellationToken token)
    {
        try
        {
            var directoryInfo = new DirectoryInfo(saveDirectory);

            if (!directoryInfo.Exists)
            {
                Logger.LogError("Failed to take backup for path {@SaveDir} because it doesn't exist", saveDirectory);

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
                    Logger.LogTrace("Backing up directory {@SaveDir}", saveDirectory);
                    ZipFile.CreateFromDirectory(saveDirectory, backupPath);

                    return Task.CompletedTask;
                });
        } catch (Exception e)
        {
            Logger.LogError(e, "Failed to take backup for path {@SaveDir}", saveDirectory);
        }
    }
}