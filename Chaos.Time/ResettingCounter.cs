using Chaos.Time.Abstractions;

namespace Chaos.Time;

public sealed class ResettingCounter : IDeltaUpdatable
{
    private readonly int MaxCount;
    private readonly IIntervalTimer Timer;
    private int Counter;

    public bool Maxed => Counter >= MaxCount;

    public ResettingCounter(int maxPerSecond, IIntervalTimer timer)
    {
        Timer = timer;
        MaxCount = maxPerSecond;
    }

    public bool TryIncrement()
    {
        if (Counter >= MaxCount)
            return false;

        Interlocked.Increment(ref Counter);

        return Counter < MaxCount;
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        Timer.Update(delta);

        if (Timer.IntervalElapsed)
            Counter = 0;
    }
}