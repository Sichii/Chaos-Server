namespace Chaos.Time.Abstractions;

public interface IIntervalTimer : IDeltaUpdatable
{
    bool IntervalElapsed { get; }
    void Reset();
}