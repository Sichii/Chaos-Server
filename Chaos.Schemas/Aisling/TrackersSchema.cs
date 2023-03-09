using System.Text.Json.Serialization;
using Chaos.Collections.Common;
using Chaos.Collections.Time;

namespace Chaos.Schemas.Aisling;

public sealed class TrackersSchema
{
    [JsonRequired]
    public CounterCollection Counters { get; set; } = null!;
    [JsonRequired]
    public EnumCollection Enums { get; set; } = null!;
    [JsonRequired]
    public FlagCollection Flags { get; set; } = null!;
    [JsonRequired]
    public TimedEventCollection TimedEvents { get; set; } = null!;
}