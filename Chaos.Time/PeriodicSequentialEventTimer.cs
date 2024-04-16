using Chaos.Time.Abstractions;

namespace Chaos.Time;

/// <summary>
///     A timer that runs a sequence of timers in order. When one timer has elapsed, the next timer begins elapsing. This
///     timer is periodic, meaning it will always update the first timer in the sequence, even if other timers are
///     elapsing.
/// </summary>
public class PeriodicSequentialEventTimer : ISequentialTimer
{
    private readonly List<IIntervalTimer> OrderedTimers;
    private int CurrentTimerIndex;

    /// <inheritdoc />
    public IIntervalTimer CurrentTimer => OrderedTimers[CurrentTimerIndex];

    /// <inheritdoc />
    public bool IntervalElapsed => OrderedTimers[CurrentTimerIndex].IntervalElapsed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PeriodicSequentialEventTimer" /> class
    /// </summary>
    public PeriodicSequentialEventTimer(params IIntervalTimer[] orderedTimers) => OrderedTimers = orderedTimers.ToList();

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

        if (timer.IntervalElapsed)
            CurrentTimerIndex++;

        if (CurrentTimerIndex >= OrderedTimers.Count)
            Reset();

        timer = OrderedTimers[CurrentTimerIndex];

        timer.Update(delta);

        // first timer always updates (periodic)
        if (CurrentTimerIndex != 0)
            OrderedTimers[0]
                .Update(delta);
    }
}