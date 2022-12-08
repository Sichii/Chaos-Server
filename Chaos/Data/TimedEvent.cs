namespace Chaos.Data;

public sealed class TimedEvent
{
    public enum TimedEventId { }

    public required TimeSpan Duration { get; init; }

    public required TimedEventId EventId { get; init; }
    public required string Name { get; init; }
    public required int Qualifier { get; init; }

    public bool Elapsed => DateTime.UtcNow - Start > Duration;
    public DateTime Start { get; } = DateTime.UtcNow;
}