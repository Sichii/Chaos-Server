#region
using Chaos.Common.Abstractions.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.WorldScripts.Abstractions;
using Chaos.Time;
#endregion

namespace Chaos.Services.Other;

public sealed class WorldScriptingService : BackgroundService
{
    private readonly ILogger<WorldScriptingService> Logger;
    private readonly IServiceProvider Provider;

    public WorldScriptingService(IServiceProvider provider, ILogger<WorldScriptingService> logger)
    {
        Provider = provider;
        Logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var serverScripts = new List<IWorldScript>();

        foreach (var type in typeof(IWorldScript).LoadImplementations())
            try
            {
                var script = (IWorldScript)ActivatorUtilities.CreateInstance(Provider, type);
                serverScripts.Add(script);
            } catch (Exception e)
            {
                Logger.LogWarning(e, "Failed to create server script of type {Type}", type.Name);
            }

        var delta = 1000.0 / 15.0;
        var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(delta));
        var deltaTime = new DeltaTime();

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await timer.WaitForNextTickAsync(stoppingToken)
                           .ConfigureAwait(false);

                var currentDelta = deltaTime.GetDelta;

                using var worldScriptUpdateActivity = ActivitySources.StartWorldScriptActivity("WorldScript.UpdateAll");
                worldScriptUpdateActivity?.SetTag("worldscript.count", serverScripts.Count);
                worldScriptUpdateActivity?.SetTag("delta_ms", currentDelta.TotalMilliseconds);

                foreach (var script in serverScripts)
                    if (script.Enabled)
                        try
                        {
                            var scriptType = script.GetType()
                                                   .Name;

                            using var updateActivity = ActivitySources.StartWorldScriptActivity($"WorldScript.Update.{scriptType}");
                            script.Update(currentDelta);
                        } catch (Exception e)
                        {
                            Logger.WithTopics(Topics.Actions.Update)
                                  .LogWarning(
                                      e,
                                      "Failed to update server script of type {Type}",
                                      script.GetType()
                                            .Name);
                        }
            } catch (OperationCanceledException)
            {
                return;
            }
    }
}