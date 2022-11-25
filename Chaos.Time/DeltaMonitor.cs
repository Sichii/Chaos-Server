using Chaos.Time.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Time;

/// <summary>
///     Monitors the execution time of a tight loop. Logs output so the used has better insight into how long execution is taking.
/// </summary>
public sealed class DeltaMonitor : IDeltaUpdatable
{
    private readonly double MaxDelta;
    private readonly ILogger Logger;
    private readonly IIntervalTimer Timer;
    private List<TimeSpan> ExecutionDeltas;
    private bool FirstPrint;

    public DeltaMonitor(ILogger logger, TimeSpan logInterval, double maxDelta)
    {
        Logger = logger;
        MaxDelta = maxDelta;
        ExecutionDeltas = new List<TimeSpan>();
        Timer = new IntervalTimer(logInterval, false);
        FirstPrint = true;
    }

    /// <summary>
    ///     Adds a recorded <see cref="System.TimeSpan"/> that represents how much time execution took
    /// </summary>
    /// <param name="executionDelta">The amount of time the loop took to execute</param>
    public void AddExecutionDelta(TimeSpan executionDelta) => ExecutionDeltas.Add(executionDelta);

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        Timer.Update(delta);

        if (Timer.IntervalElapsed)
        {
            PrintStatistics(ExecutionDeltas);
            ExecutionDeltas = new List<TimeSpan>(ExecutionDeltas.Count);
        }
    }

    private void PrintStatistics(List<TimeSpan> deltas) => _ = Task.Run(
        () =>
        {
            //the first set of statistic will be inaccurate because of JIT compilation, world loading, and other things
            if (FirstPrint)
            {
                FirstPrint = false;

                //so instead of printing the first set of statistic, print notes that give information about the monitor
                Logger.LogInformation(
                    """
Delta Monitor Notes: 
This message is shown in place of the first statistics message, because the first set of statistics are not accurate
Server may run slow at first as the JIT recompiles things
Max deltas can generally be ignored, if they go too high the message will show up as a WARN or ERROR
If the log message is INFO, then it's fine
If the log message is WARN, you may have something to worry about
If the log message is ERROR, then you've done something seriously wrong
""");

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
                var second = deltas[(count / 2) - 1];
                median = (first + second).TotalMilliseconds / 2;
            } else
                median = deltas[count / 2].TotalMilliseconds;

            //log output format
            const string FORMAT =
                "Delta Monitor - Average: {Average:N1}ms, Median: {Median:N1}ms, 95th%: {UpperPercentile:N1}, Max: {Max:N1}, Samples: {SampleCount}";

            var objs = new object[] { average, median, upperPct, max, deltas.Count };

            //depending on how the loop is performing, log the output at different levels
            if ((average > MaxDelta) || (max > 250))
                Logger.LogError(FORMAT, objs);
            else if ((upperPct > MaxDelta / 2) || (max > 100))
                Logger.LogWarning(FORMAT, objs);
            else
                Logger.LogInformation(FORMAT, objs);
        });
}