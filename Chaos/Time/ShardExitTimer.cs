using Chaos.Objects.World;
using Chaos.Time.Abstractions;

namespace Chaos.Time;

public class ShardExitTimer : IntervalTimer
{
    private readonly Aisling Aisling;
    private readonly IIntervalTimer SubIntervalTimer;
    private bool FinalWarningGiven;

    protected virtual TimeSpan Remaining => Interval - Elapsed;

    /// <inheritdoc />
    public ShardExitTimer(Aisling aisling, TimeSpan interval)
        : base(interval, false)
    {
        Aisling = aisling;
        SubIntervalTimer = new IntervalTimer(TimeSpan.FromSeconds(5), false);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        //this is a one-time-timer
        if (IntervalElapsed)
            return;

        base.Update(delta);

        var remaining = Remaining;

        if (remaining.TotalSeconds > 1)
        {
            SubIntervalTimer.Update(delta);

            if (SubIntervalTimer.IntervalElapsed)
                Aisling.SendActiveMessage($"You will be removed from the map in {(int)Math.Ceiling(remaining.TotalSeconds)} seconds");
        } else if (!FinalWarningGiven && (remaining.TotalSeconds <= 1))
        {
            FinalWarningGiven = true;
            Aisling.SendActiveMessage("You will be removed from the map in 1 second");
        }
    }
}