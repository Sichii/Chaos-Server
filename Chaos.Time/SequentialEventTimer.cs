using Chaos.Time.Abstractions;

namespace Chaos.Time;

/// <summary>
///     A timer that runs a sequence of timers in order. When one timer has elapsed, the next timer begins elapsing. When
///     the last timer has elapsed, the first timer begins again.
/// </summary>
public class SequentialEventTimer : IIntervalTimer
{
    private readonly List<IIntervalTimer> OrderedTimers;
    private int CurrentTimerIndex;

    /// <inheritdoc />
    public bool IntervalElapsed => OrderedTimers[CurrentTimerIndex].IntervalElapsed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SequentialEventTimer" /> class
    /// </summary>
    public SequentialEventTimer(params IIntervalTimer[] orderedTimers) => OrderedTimers = orderedTimers.ToList();

    /// <inheritdoc />
    public void Reset()
    {
        OrderedTimers.ForEach(timer => timer.Reset());
        CurrentTimerIndex = 0;
    }

    /// <inheritdoc />
    public void SetOrigin(DateTime origin)
        => throw new NotSupportedException("Would not make sense to set the origin of all timers in a sequence");

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        var timer = OrderedTimers[CurrentTimerIndex];
        timer.Update(delta);

        if (timer.IntervalElapsed)
            CurrentTimerIndex++;

        if (CurrentTimerIndex >= OrderedTimers.Count)
            CurrentTimerIndex = 0;
    }
}