#region
using System.Net;
using System.Text.Json.Serialization;
using Chaos.Collections.Common;
using Chaos.Collections.Time;
#endregion

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of the object that contains all trackers
/// </summary>
public class TrackersSchema
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

/// <summary>
///     Represents the serializable schema of the object that contains all trackers for an aisling
/// </summary>
public sealed class AislingTrackersSchema : TrackersSchema
{
    /// <summary>
    ///     The IP addresses associated with this aisling
    /// </summary>
    public List<string> AssociatedIpAddresses { get; set; } = [];

    /// <summary>
    ///     The last IP address used by this aisling
    /// </summary>
    public string? LastIpAddress { get; set; }

    /// <summary>
    ///     The time the aisling last logged in
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    ///     The time the aisling last logged out
    /// </summary>
    public DateTime? LastLogout { get; set; }
}