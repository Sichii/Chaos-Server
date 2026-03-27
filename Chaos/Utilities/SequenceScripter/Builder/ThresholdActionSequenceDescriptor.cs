#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Utilities.SequenceScripter.Builder;

public sealed class ThresholdActionSequenceDescriptor<T> where T: Creature
{
    public TimeSpan? DelayAfterThreshold { get; init; }
    public TimedActionSequenceDescriptor<T> Sequence { get; }
    public int Threshold { get; }

    public ThresholdActionSequenceDescriptor(int threshold, TimedActionSequenceDescriptor<T> sequence)
    {
        Threshold = threshold;
        Sequence = sequence;
    }
}