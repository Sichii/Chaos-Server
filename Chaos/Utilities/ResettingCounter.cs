using System.Threading;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Utilities;

public class ResettingCounter : IDeltaUpdatable
{
    private readonly int MaxCount;
    private readonly IIntervalTimer Timer;
    private int Counter;

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
    
    public bool Maxed => Counter >= MaxCount;
}