namespace Chaos.Time.Abstractions;

/// <summary>
///     Defines a pattern for an object that is updateable by a delta of elapsed time
/// </summary>
public interface IDeltaUpdatable
{
    /// <summary>
    ///     Updates the object with the given delta of elapsed time
    /// </summary>
    /// <param name="delta">
    ///     The amount of time that has elapsed since this method was last called
    /// </param>
    void Update(TimeSpan delta);
}