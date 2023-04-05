using System.Text.Json.Serialization;
using Chaos.Collections.Common;
using Chaos.Collections.Time;

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of the object that contains all trackers
/// </summary>
public sealed class TrackersSchema
{
    /// <summary>
    ///     A collection of counters, or string-int pairs
    /// </summary>
    [JsonRequired]
    public CounterCollection Counters { get; set; } = null!;
    /// <summary>
    ///     A collection of enums. Enums can only have one value at a time
    /// </summary>
    [JsonRequired]
    public EnumCollection Enums { get; set; } = null!;
    /// <summary>
    ///     A collection of flags. Flags can have multiple values at once
    /// </summary>
    [JsonRequired]
    public FlagCollection Flags { get; set; } = null!;
    /// <summary>
    ///     A collection of timed events.
    /// </summary>
    [JsonRequired]
    public TimedEventCollection TimedEvents { get; set; } = null!;
}