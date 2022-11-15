namespace Chaos.Common.Identity;

/// <summary>
///     A utility class used for generating unique ids in a thread-safe manner. These unique ids will not persist between
///     application restarts.
/// </summary>
public static class ClientId
{
    private static uint CurrentId;

    /// <summary>
    ///     Generates the next id in the sequence. This is thread safe.
    /// </summary>
    public static uint NextId => Interlocked.Increment(ref CurrentId);
}