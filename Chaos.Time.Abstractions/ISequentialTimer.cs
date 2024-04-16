namespace Chaos.Time.Abstractions;

/// <summary>
///     Defines a pattern for an <see cref="IIntervalTimer" /> that times a sequences of events in order. Contains multiple
///     other timers.
/// </summary>
public interface ISequentialTimer : IIntervalTimer
{
    /// <summary>
    ///     Gets the current elapsing timer
    /// </summary>
    public IIntervalTimer CurrentTimer { get; }
}