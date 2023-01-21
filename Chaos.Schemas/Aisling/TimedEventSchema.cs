using System.Text.Json.Serialization;

namespace Chaos.Schemas.Aisling;

public sealed class TimedEventSchema
{
    public bool AutoConsume { get; set; }
    public TimeSpan Duration { get; set; }
    [JsonRequired]
    public string EventId { get; set; } = null!;
    public DateTime Start { get; set; }
    public ulong UniqueId { get; set; }
}