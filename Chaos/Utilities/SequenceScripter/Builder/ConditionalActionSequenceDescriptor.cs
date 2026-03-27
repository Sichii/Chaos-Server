namespace Chaos.Utilities.SequenceScripter.Builder;

public sealed class ConditionalActionSequenceDescriptor<T>
{
    public Func<T, bool> Condition { get; }
    public TimedActionSequenceDescriptor<T> Sequence { get; }

    public ConditionalActionSequenceDescriptor(Func<T, bool> condition, TimedActionSequenceDescriptor<T> sequence)
    {
        Condition = condition;
        Sequence = sequence;
    }
}