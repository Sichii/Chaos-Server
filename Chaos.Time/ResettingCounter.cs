using Chaos.Time.Abstractions;

namespace Chaos.Time;

/// <summary>
///     A counter that resets each time a specified time period has elapsed, utilizing a delta time value
/// </summary>
public sealed class ResettingCounter : IDeltaUpdatable
{
    private readonly int MaxCount;
    private readonly IIntervalTimer Timer;
    private int Counter;

    public bool CanIncrement => Counter < MaxCount;

    public ResettingCounter(int maxPerSecond, IIntervalTimer timer)
    {
        Timer = timer;
        MaxCount = maxPerSecond;
    }

    /// <summary>
    ///     Attempts to increment the counter
    /// </summary>
    /// <returns><c>true</c> if the counter is below it's maximum value, otherwise <c>false</c></returns>
    public bool TryIncrement()
    {
        if (Counter >= MaxCount)
            return false;

        var newCounter = Interlocked.Increment(ref Counter);

        return newCounter < MaxCount;
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        Timer.Update(delta);

        if (Timer.IntervalElapsed)
            Counter = 0;
    }
}