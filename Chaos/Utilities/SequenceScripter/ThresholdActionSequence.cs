#region
using Chaos.Models.World.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities.SequenceScripter.Builder;
#endregion

namespace Chaos.Utilities.SequenceScripter;

public sealed class ThresholdActionSequence<T> where T: Creature
{
    private readonly TimedActionSequence<T> Sequence;
    private readonly int Threshold;
    private readonly IIntervalTimer? Timer;
    private bool Activated;
    private decimal PreviousValue;

    public ThresholdActionSequence(ThresholdActionSequenceDescriptor<T> descriptor)
    {
        Threshold = descriptor.Threshold;
        Sequence = new TimedActionSequence<T>(descriptor.Sequence);
        PreviousValue = 100.0m;

        if (descriptor.DelayAfterThreshold.HasValue)
            Timer = new IntervalTimer(descriptor.DelayAfterThreshold.Value);
    }

    public bool Update(T entity, TimeSpan delta)
    {
        var previousHealthPercent = PreviousValue;
        var currentHealthPercent = entity.StatSheet.HealthPercent;
        PreviousValue = currentHealthPercent;

        if ((previousHealthPercent > Threshold) && (Threshold >= currentHealthPercent))
            Activated = true;

        if (!Activated)
            return false;

        if (Timer is not null)
        {
            Timer.Update(delta);

            if (!Timer.IntervalElapsed)
                return false;
        }

        Sequence.Update(entity, delta);

        return true;
    }
}