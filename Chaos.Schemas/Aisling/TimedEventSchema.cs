using System.Text.Json.Serialization;

namespace Chaos.Schemas.Aisling;

public sealed class TimedEventSchema
{
    public bool AutoConsume { get; init; }
    public TimeSpan Duration { get; init; }
    [JsonRequired]
    public string EventId { get; init; } = null!;
    public DateTime Start { get; init; }
    public ulong UniqueId { get; init; }
}