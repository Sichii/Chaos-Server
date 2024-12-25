#region
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Time;

/// <summary>
///     A counter that resets each time a specified time period has elapsed, utilizing a delta time value
/// </summary>
public sealed class ResettingCounter : IDeltaUpdatable
{
    private readonly IIntervalTimer Timer;
    private readonly int UpdateIntervalSecs;
    private int Counter;
    private int MaxCount { get; set; }

    /// <summary>
    ///     Gets whether or not the counter can be incremented
    /// </summary>
    public bool CanIncrement => Counter < MaxCount;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResettingCounter" /> class
    /// </summary>
    /// <param name="maxCount">
    ///     The maximum value of the counter
    /// </param>
    /// <param name="timer">
    ///     The timer to use internally to determine when to reset the counter
    /// </param>
    public ResettingCounter(int maxCount, IIntervalTimer timer)
    {
        Timer = timer;
        MaxCount = maxCount;
        UpdateIntervalSecs = 1;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResettingCounter" /> class
    /// </summary>
    /// <param name="maxPerSecond">
    ///     The max acceptable increments per second. Note that this is not necessarily enforced per second, but instead
    ///     calculates a max value for the number of seconds in each interval
    /// </param>
    /// <param name="updateIntervalSecs">
    ///     The number of seconds that must elapse before resetting the counter
    /// </param>
    public ResettingCounter(int maxPerSecond, int updateIntervalSecs = 1)
    {
        Timer = new IntervalTimer(TimeSpan.FromSeconds(updateIntervalSecs));
        MaxCount = maxPerSecond * updateIntervalSecs;
        UpdateIntervalSecs = updateIntervalSecs;
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        Timer.Update(delta);

        if (Timer.IntervalElapsed)
            Counter = 0;
    }

    /// <summary>
    ///     Resets the counter back to 0
    /// </summary>
    public void Reset() => Counter = 0;

    /// <summary>
    ///     Sets the maximum count of the counter (will be automatically multiplied by the update interval)
    /// </summary>
    /// <param name="maxCount">
    ///     The new MaxCount to use
    /// </param>
    public void SetMaxCount(int maxCount) => MaxCount = maxCount * UpdateIntervalSecs;

    /// <summary>
    ///     Attempts to increment the counter
    /// </summary>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the counter is below it's maximum value, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool TryIncrement()
    {
        if (Counter >= MaxCount)
            return false;

        var newCounter = Interlocked.Increment(ref Counter);

        return newCounter <= MaxCount;
    }
}