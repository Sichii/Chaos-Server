#region
using Chaos.Utilities.SequenceScripter.Builder;
#endregion

namespace Chaos.Utilities.SequenceScripter;

public sealed class ConditionalActionSequence<T>
{
    public Func<T, bool> Condition { get; }
    public TimedActionSequence<T> Sequence { get; }

    public ConditionalActionSequence(ConditionalActionSequenceDescriptor<T> descriptor)
    {
        Condition = descriptor.Condition;
        Sequence = new TimedActionSequence<T>(descriptor.Sequence);
    }

    public bool Update(T entity, TimeSpan delta)
    {
        if (!Condition(entity))
            return false;

        Sequence.Update(entity, delta);

        return true;
    }
}