using Chaos.Extensions.Common;
using Chaos.Time.Abstractions;

namespace Chaos.Time;

public class PeriodicMessageTimer : IntervalTimer
{
    private readonly TimeSpan IntervalTransitionTime;
    private readonly Action<string> MessageAction;
    private readonly string MessageFormat;
    private readonly TimeSpan SubInterval;
    private readonly IIntervalTimer SubIntervalTimer;
    private readonly TimeSpan TransitionedInterval;
    private readonly IIntervalTimer TransitionedIntervalTimer;
    private TimeSpan CleanTimeSpan;
    private bool FinalWarningGiven;

    protected virtual TimeSpan Remaining => Interval - Elapsed;

    /// <inheritdoc />
    public PeriodicMessageTimer(
        TimeSpan length,
        TimeSpan subInterval,
        TimeSpan intervalTransitionTime,
        TimeSpan transitionedInterval,
        string messageFormat,
        Action<string> messageAction)
        : base(length, false)
    {
        SubIntervalTimer = new IntervalTimer(subInterval, false);
        IntervalTransitionTime = intervalTransitionTime;
        TransitionedIntervalTimer = new IntervalTimer(transitionedInterval, false);
        MessageFormat = messageFormat;
        MessageAction = messageAction;
        CleanTimeSpan = length;
        TransitionedInterval = transitionedInterval;
        SubInterval = subInterval;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        //this is a one-time-timer
        if (IntervalElapsed)
            return;

        base.Update(delta);

        if (FinalWarningGiven)
            return;

        var remaining = Remaining;

        if (remaining > IntervalTransitionTime)
        {
            SubIntervalTimer.Update(delta);

            if (SubIntervalTimer.IntervalElapsed)
            {
                CleanTimeSpan -= SubInterval;
                var messageActual = MessageFormat.Inject(CleanTimeSpan.ToReadableString());
                MessageAction(messageActual);
            }
        } else
        {
            if (CleanTimeSpan > IntervalTransitionTime)
            {
                CleanTimeSpan = IntervalTransitionTime;
                var messageActual = MessageFormat.Inject(CleanTimeSpan.ToReadableString());
                MessageAction(messageActual);
            }

            TransitionedIntervalTimer.Update(delta);

            if (TransitionedIntervalTimer.IntervalElapsed)
            {
                CleanTimeSpan -= TransitionedInterval;
                var messageActual = MessageFormat.Inject(CleanTimeSpan.ToReadableString());
                MessageAction(messageActual);
            }

            if (CleanTimeSpan <= TransitionedInterval)
                FinalWarningGiven = true;
        }
    }
}