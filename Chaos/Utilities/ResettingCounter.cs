using System.Threading;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Utilities;

public class ResettingCounter : IDeltaUpdatable
{
    private readonly int MaxCount;
    private readonly IIntervalTimer Timer;
    private int Counter;

    public ResettingCounter(int maxCount)
    {
        Timer = new IntervalTimer(TimeSpan.FromSeconds(1));
        MaxCount = maxCount;
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