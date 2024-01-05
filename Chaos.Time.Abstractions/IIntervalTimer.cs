namespace Chaos.Time.Abstractions;

/// <summary>
///     Defines a pattern for a timer that utilizes a time delta to increment the timer
/// </summary>
public interface IIntervalTimer : IDeltaUpdatable
{
    /// <summary>
    ///     Whether or not the timer has elapsed
    /// </summary>
    bool IntervalElapsed { get; }

    /// <summary>
    ///     Resets the timer
    /// </summary>
    void Reset();

    /// <summary>
    ///     Sets the origin of the timer, allowing you to base your interval off a specific timer
    /// </summary>
    /// <param name="origin">
    ///     The time to emulate that the timer has originally started
    /// </param>
    void SetOrigin(DateTime origin);
}