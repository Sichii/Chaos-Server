using System.Diagnostics;
using Chaos.Common.Abstractions;
using Chaos.IO.FileSystem;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage.Abstractions;

public interface IPeriodicSaveStoreOptions : IDirectoryBound
{
    string Directory { get; set; }
    int SaveIntervalMins { get; set; }
}

public abstract class PeriodicSaveStoreBase<T, TOptions> : BackgroundService, IStore<T> where TOptions: class, IPeriodicSaveStoreOptions
{
    protected ConcurrentDictionary<string, T> Cache { get; }
    protected IEntityRepository EntityRepository { get; }
    protected ILogger Logger { get; }
    protected TOptions Options { get; }

    protected PeriodicSaveStoreBase(IEntityRepository entityRepository, IOptions<TOptions> options, ILogger logger)
    {
        EntityRepository = entityRepository;
        Options = options.Value;
        Logger = logger;
        Cache = new ConcurrentDictionary<string, T>(StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    /// <inheritdoc />
    public virtual bool Exists(string key)
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
    public virtual T Load(string key)
    {
        var directory = Path.Combine(Options.Directory, key);

        // ReSharper disable once HeapView.CanAvoidClosure
        T InnerLoad(string dir) => Cache.GetOrAdd(key, _ => LoadFromFile(directory, key));

        return directory.SafeExecute(InnerLoad);
    }

    /// <inheritdoc />
    public virtual bool Remove(string key)
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
    public abstract void Save(T obj);

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var saveTimer = new PeriodicTimer(TimeSpan.FromMinutes(Options.SaveIntervalMins));

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await saveTimer.WaitForNextTickAsync(stoppingToken);
                var start = Stopwatch.GetTimestamp();

                Logger.LogDebug("Performing save");
                var mailBoxes = Cache.Values.ToList();

                await Task.WhenAll(mailBoxes.Select(SaveAsync));

                Logger.LogDebug("Save completed, took {@Elapsed}", Stopwatch.GetElapsedTime(start));
            } catch (OperationCanceledException)
            {
                //ignore
                break;
            } catch (Exception e)
            {
                Logger.LogCritical(e, "Exception while performing save");
            }

        Logger.LogInformation("Performing final save before shutdown");

        var guildsToSave = Cache.Values.ToList();
        await Task.WhenAll(guildsToSave.Select(SaveAsync));

        Logger.LogInformation("Final save completed");
    }

    protected abstract T LoadFromFile(string dir, string key);

    public abstract Task SaveAsync(T obj);
}