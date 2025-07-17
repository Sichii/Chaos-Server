#region
using Chaos.Models.World.Abstractions;
using Chaos.Utilities.SequenceScripter.Builder;
#endregion

namespace Chaos.Utilities.SequenceScripter;

public sealed class ConditionalTimedActionSequence<T> where T: Creature
{
    public Func<T, bool> Condition { get; }
    public TimedActionSequence<T> Sequence { get; }

    public ConditionalTimedActionSequence(ConditionalTimedActionSequenceDescriptor<T> descriptor)
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