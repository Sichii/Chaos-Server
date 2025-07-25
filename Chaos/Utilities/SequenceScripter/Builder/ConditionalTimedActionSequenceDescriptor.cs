#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Utilities.SequenceScripter.Builder;

public class ConditionalTimedActionSequenceDescriptor<T> where T: Creature
{
    public Func<T, bool> Condition { get; }
    public TimedActionSequenceDescriptor<T> Sequence { get; }

    public ConditionalTimedActionSequenceDescriptor(Func<T, bool> condition, TimedActionSequenceDescriptor<T> sequence)
    {
        Condition = condition;
        Sequence = sequence;
    }
}