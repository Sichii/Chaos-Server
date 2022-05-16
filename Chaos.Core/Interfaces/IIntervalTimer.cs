namespace Chaos.Core.Interfaces;

public interface IIntervalTimer : IDeltaUpdatable
{
    bool IntervalElapsed { get; }
}