namespace Chaos.Utilities.SequenceScripter.Builder;

public sealed class TimedActionSequenceDescriptor<T>
{
    public List<TimedActionDescriptor<T>> Sequence = [];
    public TimeSpan? StartingAtTime { get; init; }
}