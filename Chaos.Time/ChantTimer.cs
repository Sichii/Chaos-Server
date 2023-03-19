using Chaos.Time.Abstractions;

namespace Chaos.Time;

/// <summary>
///     Keeps track of when spell chants start and end, and validates whether or not a spell should be allowed to finish casting.
/// </summary>
public sealed class ChantTimer : IDeltaUpdatable
{
    private readonly int MaxTimeBurdenMs;
    private int ElapsedMs;
    private int ExpectedCastLines;
    private int RemainingChantTimeMs;
    private int TimeBurdenMs;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChantTimer" /> class
    /// </summary>
    /// <param name="maxTimeBurdenMs">The maximum number of milliseconds the spell can be late before being canceled</param>
    public ChantTimer(int maxTimeBurdenMs) => MaxTimeBurdenMs = maxTimeBurdenMs;

    /// <summary>
    ///     Starts a chant with the given number of expected cast lines
    /// </summary>
    /// <param name="castLines">The number of cast lines received from the client. This value is not to be fully trusted.</param>
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

    /// <summary>
    ///     Valides that a spell chant was valid and was completed in approximately the expected amount of time.
    /// </summary>
    /// <param name="castLines">The number of cast lines the spell should have had. This value is trustable.</param>
    /// <returns><c>true</c> if the spell cast is valid and finished in approximately the expected amount of time, otherwise <c>false</c></returns>
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