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
    private readonly IIntervalTimer Timer;

    public TimedAction(TimedActionDescriptor<T> descriptor)
    {
        Timer = new IntervalTimer(descriptor.Time, descriptor.StartAsElapsed);
        Action = descriptor.Action;
    }

    public bool Update(T entity, TimeSpan delta)
    {
        Timer.Update(delta);

        if (!Timer.IntervalElapsed)
            return false;

        Action(entity);

        return true;
    }
}