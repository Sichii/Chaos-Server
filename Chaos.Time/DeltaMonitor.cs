using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Time.Abstractions;
using Microsoft.Extensions.Logging;
using TDigestNet;

namespace Chaos.Time;

/// <summary>
///     Monitors the execution time of a tight loop. Logs output so the used has better insight into how long execution is
///     taking.
/// </summary>
public sealed class DeltaMonitor : IDeltaUpdatable
{
    private readonly ILogger Logger;
    private readonly double MaxDelta;
    private readonly string Name;
    private readonly IIntervalTimer Timer;
    private bool BeginLogging;
    private TDigest Digest;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeltaMonitor" /> class
    /// </summary>
    /// <param name="name">
    ///     The name of this instance
    /// </param>
    /// <param name="logger">
    ///     An object to log with
    /// </param>
    /// <param name="logInterval">
    ///     How often to log
    /// </param>
    /// <param name="maxDelta">
    ///     The maximum acceptable delta allowed before logging an error
    /// </param>
    public DeltaMonitor(
        string name,
        ILogger logger,
        TimeSpan logInterval,
        double maxDelta)
    {
        Name = name;
        Logger = logger;
        MaxDelta = maxDelta;
        Digest = new TDigest();
        Timer = new IntervalTimer(logInterval, false);
        BeginLogging = false;
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        Timer.Update(delta);

        if (Timer.IntervalElapsed)
        {
            PrintStatistics(Digest);
            Digest = new TDigest();
        }
    }

    /// <summary>
    ///     Adds a recorded <see cref="System.TimeSpan" /> that represents how much time execution took
    /// </summary>
    /// <param name="executionDelta">
    ///     The amount of time the loop took to execute
    /// </param>
    public void DigestDelta(TimeSpan executionDelta) => Digest.Add(executionDelta.Ticks);

    /// <summary>
    ///     Analyzes the recorded deltas and logs the results
    /// </summary>
    private void PrintStatistics(TDigest digest)
        => _ = Task.Run(
            () =>
            {
                if (!BeginLogging)
                {
                    BeginLogging = true;

                    return Task.CompletedTask;
                }

                //gather various statistics about the deltas
                var average = digest.Average / TimeSpan.TicksPerMillisecond;
                var max = digest.Max / TimeSpan.TicksPerMillisecond;
                var count = digest.Count;
                var upperPct = digest.Quantile(0.95) / TimeSpan.TicksPerMillisecond;
                var median = digest.Quantile(0.5) / TimeSpan.TicksPerMillisecond;

                //log output format
                const string FORMAT
                    = "Delta Monitor [{Name}] - Average: {Average:N1}ms, Median: {Median:N1}ms, 95th%: {UpperPercentile:N1}ms, Max: {Max:N1}ms, Samples: {SampleCount}";

                //depending on how the loop is performing, log the output at different levels
                if ((average > MaxDelta) || (max > 250))
                    Logger.WithTopics(Topics.Entities.DeltaMonitor, Topics.Actions.Update)
                          .LogError(
                              FORMAT,
                              Name,
                              average,
                              median,
                              upperPct,
                              max,
                              count);
                else if ((upperPct > (MaxDelta / 2)) || (max > 100))
                    Logger.WithTopics(Topics.Entities.DeltaMonitor, Topics.Actions.Update)
                          .LogWarning(
                              FORMAT,
                              Name,
                              average,
                              median,
                              upperPct,
                              max,
                              count);
                else
                    Logger.WithTopics(Topics.Entities.DeltaMonitor, Topics.Actions.Update)
                          .LogTrace(
                              FORMAT,
                              Name,
                              average,
                              median,
                              upperPct,
                              max,
                              count);

                return Task.CompletedTask;
            });
}