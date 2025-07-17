#region
using Chaos.Models.World.Abstractions;
using Chaos.Utilities.SequenceScripter.Builder;
#endregion

namespace Chaos.Utilities.SequenceScripter;

public sealed class ThresholdAction<T> where T: Creature
{
    private readonly Action<T> Action;
    private readonly int Threshold;
    private decimal PreviousValue;

    public ThresholdAction(ThresholdActionDescriptor<T> descriptor)
    {
        Threshold = descriptor.Threshold;
        Action = descriptor.Action;
        PreviousValue = 100.0m;
    }

    public bool Update(T entity, TimeSpan delta)
    {
        //already passed the threshold
        //also prevents threshold events from happening multiple times if entity is healed
        if (PreviousValue < Threshold)
            return false;

        var previousHealthPercent = PreviousValue;
        var currentHealthPercent = entity.StatSheet.HealthPercent;
        PreviousValue = currentHealthPercent;

        if ((previousHealthPercent > Threshold) && (Threshold >= currentHealthPercent))
        {
            Action(entity);

            return true;
        }

        return false;
    }
}