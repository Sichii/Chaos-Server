using Chaos.Common.Identity;

namespace Chaos.Data;

public sealed class TimedEvent : IEquatable<TimedEvent>
{
    public enum TimedEventId
    {
        Fountain
    }

    public bool Completed => DateTime.UtcNow - Start > Duration;
    public TimeSpan Duration { get; }
    public TimedEventId EventId { get; }
    public DateTime Start { get; }

    public ulong UniqueId { get; }

    public TimedEvent(
        ulong uniqueId,
        TimedEventId eventId,
        TimeSpan duration,
        DateTime start
    )
    {
        UniqueId = uniqueId;
        EventId = eventId;
        Duration = duration;
        Start = start;
    }

    public TimedEvent(TimedEventId eventId, TimeSpan duration)
    {
        UniqueId = ServerId.NextId;
        EventId = eventId;
        Duration = duration;
        Start = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public bool Equals(TimedEvent? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return UniqueId == other.UniqueId;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is TimedEvent other && Equals(other));

    /// <inheritdoc />
    public override int GetHashCode() => UniqueId.GetHashCode();
}