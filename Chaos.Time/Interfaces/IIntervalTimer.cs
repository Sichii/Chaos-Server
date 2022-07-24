namespace Chaos.Time.Interfaces;

public interface IIntervalTimer : IDeltaUpdatable
{
    bool IntervalElapsed { get; }
}