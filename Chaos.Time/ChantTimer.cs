using Chaos.Time.Abstractions;

namespace Chaos.Time;

public sealed class ChantTimer : IDeltaUpdatable
{
    private readonly int MaxTimeBurdenMs;
    private int ElapsedMs;
    private int ExpectedCastLines;
    private int RemainingChantTimeMs;
    private int TimeBurdenMs;

    public ChantTimer(int maxTimeBurdenMs) => MaxTimeBurdenMs = maxTimeBurdenMs;

    public void Start(byte castLines)
    {
        ElapsedMs = 0;
        ExpectedCastLines = castLines;
        RemainingChantTimeMs = castLines * 1000;
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        var elapsedMs = (int)delta.TotalMilliseconds;
        ElapsedMs += elapsedMs;
        Interlocked.Add(ref RemainingChantTimeMs, -elapsedMs);

        if (RemainingChantTimeMs < 0)
        {
            Interlocked.Add(ref TimeBurdenMs, -RemainingChantTimeMs);
            RemainingChantTimeMs = 0;

            if (TimeBurdenMs < 0)
                TimeBurdenMs = 0;
        }
    }

    public bool Validate(byte castLines)
    {
        //if the cast lines of the spell being cast are more than the expected count, the chant is invalid
        if (castLines > ExpectedCastLines)
            return false;

        //if the time burden is greater than the max time burden, the chant is invalid
        if (TimeBurdenMs > MaxTimeBurdenMs)
        {
            //subtract the elapsed time from the time burden
            Interlocked.Add(ref TimeBurdenMs, -ElapsedMs);

            if (TimeBurdenMs < 0)
                TimeBurdenMs = 0;

            return false;
        }

        //set remaining time to 0
        var remainingTimeMs = Interlocked.Exchange(ref RemainingChantTimeMs, 0);
        //add any remaining time to the time burden
        Interlocked.Add(ref TimeBurdenMs, remainingTimeMs);
        //set expected cast lines to 0 so that future spells can't ignore chant timer
        //this value must be set to a non-zero value again before another spell can pass validation
        ExpectedCastLines = 0;

        return true;
    }
}