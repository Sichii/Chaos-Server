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
    private readonly decimal UpdateIntervalSecs;
    private int _counter;

    private int MaxCount { get; set; }

    /// <summary>
    ///     Gets whether or not the counter can be incremented
    /// </summary>
    public bool CanIncrement => Counter < MaxCount;

    /// <summary>
    ///     The current value of the counter
    /// </summary>
    public int Counter => _counter;

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
    public ResettingCounter(decimal maxPerSecond, decimal updateIntervalSecs = 1)
    {
        Timer = new IntervalTimer(TimeSpan.FromSeconds((double)updateIntervalSecs));
        MaxCount = (int)Math.Ceiling(maxPerSecond * updateIntervalSecs);
        UpdateIntervalSecs = updateIntervalSecs;
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        Timer.Update(delta);

        if (Timer.IntervalElapsed)
            _counter = 0;
    }

    /// <summary>
    ///     Resets the counter back to 0
    /// </summary>
    public void Reset() => _counter = 0;

    /// <summary>
    ///     Updates the maximum count value of the counter
    /// </summary>
    /// <param name="maxCount">
    ///     The new maximum value to set for the counter
    /// </param>
    public void SetMaxCount(int maxCount) => MaxCount = maxCount;

    /// <summary>
    ///     Sets the maximum count of the counter (will be automatically multiplied by the update interval)
    /// </summary>
    /// <param name="maxPerSecond">
    ///     The max per second, which will get automatically multiplied by the update interval
    /// </param>
    public void SetMaxPerSecond(int maxPerSecond) => MaxCount = (int)Math.Ceiling(maxPerSecond * UpdateIntervalSecs);

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

        var newCounter = Interlocked.Increment(ref _counter);

        return newCounter <= MaxCount;
    }
}