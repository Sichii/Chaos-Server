using System.Diagnostics;

namespace Chaos.Core.Utilities;

/// <summary>
///     Provides an easy way to obtain a high-precision time-based value.
/// </summary>
public static class DeltaTime
{
    private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();
    private static long LastValue;

    /// <summary>
    ///     Gets the current time value in milliseconds.
    /// </summary>
    public static long Elapsed
    {
        get
        {
            var newValue = Stopwatch.ElapsedMilliseconds - LastValue;
            LastValue = newValue;

            return newValue;
        }
    }
}