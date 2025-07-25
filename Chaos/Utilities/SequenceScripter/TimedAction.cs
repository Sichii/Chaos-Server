#region
using Chaos.Models.World.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities.SequenceScripter.Builder;
#endregion

namespace Chaos.Utilities.SequenceScripter;

public sealed class TimedAction<T> where T: Creature
{
    private readonly Action<T> Action;
    private readonly int? StartingAtHealthPercent;
    private readonly IIntervalTimer Timer;
    private bool IsStarted;

    public TimedAction(TimedActionDescriptor<T> descriptor)
    {
        Timer = new IntervalTimer(descriptor.Time, descriptor.StartAsElapsed);
        Action = descriptor.Action;
        StartingAtHealthPercent = descriptor.StartingAtHealthPercent;

        if (!StartingAtHealthPercent.HasValue)
            IsStarted = true;
    }

    public bool Update(T entity, TimeSpan delta)
    {
        if (!IsStarted && (entity.StatSheet.HealthPercent < StartingAtHealthPercent!.Value))
            IsStarted = true;

        Timer.Update(delta);

        if (!Timer.IntervalElapsed)
            return false;

        Action(entity);

        return true;
    }
}