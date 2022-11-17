using Chaos.Time.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Time;

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
            if (FirstPrint)
            {
                FirstPrint = false;

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

            deltas.Sort();
            var average = deltas.Average(d => d.TotalMilliseconds);
            var max = deltas.Last().TotalMilliseconds;
            var count = deltas.Count;
            var upperPct = deltas[(int)(count * 0.95)].TotalMilliseconds;
            
            double median;

            if (count % 2 == 0)
            {
                var first = deltas[count / 2];
                var second = deltas[(count / 2) - 1];
                median = (first + second).TotalMilliseconds / 2;
            } else
                median = deltas[count / 2].TotalMilliseconds;

            const string FORMAT =
                "Delta Monitor - Average: {Average:N1}ms, Median: {Median:N1}ms, 95th%: {UpperPercentile:N1}, Max: {Max:N1}, Samples: {SampleCount}";

            var objs = new object[] { average, median, upperPct, max, deltas.Count };

            if ((average > MaxDelta) || (max > 250))
                Logger.LogError(FORMAT, objs);
            else if ((upperPct > MaxDelta / 2) || (max > 100))
                Logger.LogWarning(FORMAT, objs);
            else
                Logger.LogInformation(FORMAT, objs);
        });
}