#region
using Chaos.Models.World.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities.SequenceScripter.Builder;
#endregion

namespace Chaos.Utilities.SequenceScripter;

public class TimedActionSequence<T> where T: Creature
{
    private readonly List<Action<T>> Actions;
    private readonly IIntervalTimer? InitialTimer;
    private readonly int? StartingAtHealthPercent;
    private readonly List<IIntervalTimer> Timers;
    private int CurrentIndex;
    private bool InitialTimerExpired;
    private decimal PreviousHealthPercent;

    public TimedActionSequence(TimedActionSequenceDescriptor<T> descriptor)
    {
        StartingAtHealthPercent = descriptor.StartingAtHealthPercent;
        InitialTimer = descriptor.StartingAtTime is not null ? new IntervalTimer(descriptor.StartingAtTime.Value) : null;
        Actions = [];
        Timers = [];
        PreviousHealthPercent = 100;

        foreach (var timedAction in descriptor.Sequence)
        {
            Actions.Add(timedAction.Action);
            Timers.Add(new IntervalTimer(timedAction.Time, timedAction.StartAsElapsed));
        }
    }

    private void Reset()
    {
        CurrentIndex = 0;
        InitialTimerExpired = false;
        PreviousHealthPercent = 100;

        foreach (var timer in Timers)
            timer.Reset();
    }

    public bool Update(T entity, TimeSpan delta)
    {
        if (StartingAtHealthPercent.HasValue)
        {
            var previousHealthPercent = PreviousHealthPercent;
            var currentHealthPercent = entity.StatSheet.HealthPercent;
            PreviousHealthPercent = currentHealthPercent;

            if ((previousHealthPercent <= StartingAtHealthPercent.Value) && (currentHealthPercent > StartingAtHealthPercent.Value))
                return false;
        }

        if (InitialTimer is not null && !InitialTimerExpired)
        {
            InitialTimer.Update(delta);

            if (!InitialTimer.IntervalElapsed)
                return false;

            InitialTimerExpired = true;
        }

        var timer = Timers[CurrentIndex];
        timer.Update(delta);

        if (!timer.IntervalElapsed)
            return false;

        Actions[CurrentIndex](entity);
        CurrentIndex++;

        if (CurrentIndex >= Timers.Count)
        {
            Reset();

            return true;
        }

        return false;
    }
}