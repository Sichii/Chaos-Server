#region
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Time;

/// <summary>
///     A timer that runs a sequence of timers in order. When one timer has elapsed, the next timer begins elapsing. When
///     the last timer has elapsed, the first timer begins again.
/// </summary>
public class SequentialEventTimer : ISequentialTimer
{
    private readonly IReadOnlyList<IIntervalTimer> OrderedTimers;
    private int CurrentTimerIndex;

    /// <inheritdoc />
    public IIntervalTimer CurrentTimer => OrderedTimers[CurrentTimerIndex];

    /// <inheritdoc />
    public bool IntervalElapsed => OrderedTimers[CurrentTimerIndex].IntervalElapsed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SequentialEventTimer" /> class
    /// </summary>
    public SequentialEventTimer(params IReadOnlyList<IIntervalTimer> orderedTimers) => OrderedTimers = orderedTimers;

    /// <inheritdoc />
    public void Reset()
    {
        foreach (var timer in OrderedTimers)
            timer.Reset();

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
    }
}