using Chaos.Time.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Time;

/// <summary>
///     Monitors the execution time of a tight loop. Logs output so the used has better insight into how long execution is taking.
/// </summary>
public sealed class DeltaMonitor : IDeltaUpdatable
{
    private readonly ILogger Logger;
    private readonly double MaxDelta;
    private readonly string Name;
    private readonly IIntervalTimer Timer;
    private bool BeginLogging;
    private List<TimeSpan> ExecutionDeltas;

    public DeltaMonitor(
        string name,
        ILogger logger,
        TimeSpan logInterval,
        double maxDelta
    )
    {
        Name = name;
        Logger = logger;
        MaxDelta = maxDelta;
        ExecutionDeltas = new List<TimeSpan>();
        Timer = new IntervalTimer(logInterval, false);
        BeginLogging = false;
    }

    /// <summary>
    ///     Adds a recorded <see cref="System.TimeSpan"/> that represents how much time execution took
    /// </summary>
    /// <param name="executionDelta">The amount of time the loop took to execute</param>
    public void AddExecutionDelta(TimeSpan executionDelta) => ExecutionDeltas.Add(executionDelta);

    private void CheckStatistics(List<TimeSpan> deltas) => _ = Task.Run(
        async () =>
        {
            await Task.Yield();

            if (!BeginLogging)
            {
                BeginLogging = true;

                return;
            }

            //sort the deltas from smallest to largest
            deltas.Sort();

            //gather various statistics about the deltas
            var average = deltas.Average(d => d.TotalMilliseconds);
            var max = deltas.Last().TotalMilliseconds;
            var count = deltas.Count;
            var upperPct = deltas[(int)(count * 0.95)].TotalMilliseconds;

            double median;

            //median calculation
            if (count % 2 == 0)
            {
                var first = deltas[count / 2];
                var second = deltas[count / 2 - 1];
                median = (first + second).TotalMilliseconds / 2;
            } else
                median = deltas[count / 2].TotalMilliseconds;

            //log output format
            const string FORMAT =
                "Delta Monitor [{Name}] - Average: {Average:N1}ms, Median: {Median:N1}ms, 95th%: {UpperPercentile:N1}, Max: {Max:N1}, Samples: {SampleCount}";

            //depending on how the loop is performing, log the output at different levels
            if ((average > MaxDelta) || (max > 250))
                Logger.LogError(
                    FORMAT,
                    Name,
                    average,
                    median,
                    upperPct,
                    max,
                    deltas.Count);
            else if ((upperPct > MaxDelta / 2) || (max > 100))
                Logger.LogWarning(
                    FORMAT,
                    Name,
                    average,
                    median,
                    upperPct,
                    max,
                    deltas.Count);
            else
                Logger.LogTrace(
                    FORMAT,
                    Name,
                    average,
                    median,
                    upperPct,
                    max,
                    deltas.Count);
        });

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        Timer.Update(delta);

        if (Timer.IntervalElapsed)
        {
            CheckStatistics(ExecutionDeltas);
            ExecutionDeltas = new List<TimeSpan>(ExecutionDeltas.Count);
        }
    }
}