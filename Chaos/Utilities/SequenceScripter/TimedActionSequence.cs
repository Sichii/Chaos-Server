#region
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities.SequenceScripter.Builder;
#endregion

namespace Chaos.Utilities.SequenceScripter;

public sealed class TimedActionSequence<T>
{
    private readonly List<Action<T>> Actions;
    private readonly IIntervalTimer? InitialTimer;
    private readonly List<IIntervalTimer> Timers;
    private int CurrentIndex;
    private bool InitialTimerExpired;

    public TimedActionSequence(TimedActionSequenceDescriptor<T> descriptor)
    {
        InitialTimer = descriptor.StartingAtTime is not null ? new IntervalTimer(descriptor.StartingAtTime.Value) : null;
        Actions = [];
        Timers = [];

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

        foreach (var timer in Timers)
            timer.Reset();
    }

    public bool Update(T entity, TimeSpan delta)
    {
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