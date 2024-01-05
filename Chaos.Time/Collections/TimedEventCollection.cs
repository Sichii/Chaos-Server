using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Chaos.Common.Synchronization;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Time.Converters;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Time;

/// <summary>
///     A thread-safe, delta-updateable, and serializable collection of timed events
/// </summary>
[JsonConverter(typeof(TimedEventCollectionConverter))]
public sealed class TimedEventCollection : IEnumerable<KeyValuePair<string, TimedEventCollection.Event>>, IDeltaUpdatable
{
    private readonly Dictionary<string, Event> Events;
    private readonly IIntervalTimer Interval;
    private readonly AutoReleasingMonitor Sync;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TimedEventCollection" /> class
    /// </summary>
    /// <param name="events">
    ///     Optional initial events to populate the collection with
    /// </param>
    public TimedEventCollection(IEnumerable<Event>? events = null)
    {
        events ??= Array.Empty<Event>();
        Events = new Dictionary<string, Event>(StringComparer.OrdinalIgnoreCase);
        Sync = new AutoReleasingMonitor();
        Interval = new IntervalTimer(TimeSpan.FromSeconds(1));

        foreach (var timedEvent in events)
            Events.Add(timedEvent.EventId, timedEvent);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, Event>> GetEnumerator()
    {
        List<KeyValuePair<string, Event>> snapShot;

        using (Sync.Enter())
            snapShot = Events.ToList();

        return snapShot.GetEnumerator();
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        Interval.Update(delta);

        if (Interval.IntervalElapsed)
        {
            using var sync = Sync.Enter();

            foreach (var kvp in Events.ToList())
            {
                if (!kvp.Value.AutoConsume)
                    continue;

                if (kvp.Value.Completed)
                    Events.Remove(kvp.Key);
            }
        }
    }

    /// <summary>
    ///     Adds an event to the collection
    /// </summary>
    /// <param name="eventId">
    ///     The id of the event
    /// </param>
    /// <param name="duration">
    ///     The duration of the event
    /// </param>
    /// <param name="autoConsume">
    ///     Whether or not the event should be automatically removed from the collection when it has expired
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     An event with the same ID already exists
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Event ID cannot be null or empty
    /// </exception>
    public void AddEvent(string eventId, TimeSpan duration, bool autoConsume = false)
    {
        using var sync = Sync.Enter();

        var timedEvent = new Event(eventId, duration, autoConsume);

        if (string.IsNullOrEmpty(timedEvent.EventId))
            throw new ArgumentException("Event ID cannot be null or empty", nameof(timedEvent));

        if (!Events.TryAdd(timedEvent.EventId, timedEvent))
            throw new InvalidOperationException("An event with the same ID already exists");
    }

    /// <summary>
    ///     Attempts to retreive an active event
    /// </summary>
    /// <param name="eventId">
    ///     The id of the event to check if is active
    /// </param>
    /// <param name="event">
    ///     If an event with the given id was found and is not completed, this will be that event
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if an event was found with the given ID and that event was not completed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <remarks>
    ///     Use this when you are checking for an event that has AutoConsume=true
    /// </remarks>
    public bool HasActiveEvent(string eventId, [MaybeNullWhen(false)] out Event @event)
    {
        using var sync = Sync.Enter();

        if (!Events.TryGetValue(eventId, out @event))
            return false;

        return !@event.Completed;
    }

    /// <summary>
    ///     Attempts to consume an event
    /// </summary>
    /// <param name="eventId">
    ///     The id of the event to consume
    /// </param>
    /// <param name="event">
    ///     If an event with the given Id was present and completed, this will be that event
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if an event was found with the given ID and that event was in a completed state, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool TryConsumeEvent(string eventId, [MaybeNullWhen(false)] out Event @event)
    {
        using var sync = Sync.Enter();

        @event = null;

        if (!Events.TryGetValue(eventId, out var existingEvent))
            return false;

        if (existingEvent.Completed)
        {
            @event = existingEvent;

            return Events.Remove(eventId);
        }

        return false;
    }

    /// <summary>
    ///     An event with timing information
    /// </summary>
    public sealed class Event : IEquatable<Event>
    {
        /// <summary>
        ///     Whether or not the event should be automatically removed from the collection when it has expired
        /// </summary>
        [JsonInclude]
        public bool AutoConsume { get; }

        /// <summary>
        ///     The duration of the event
        /// </summary>
        [JsonInclude]
        public TimeSpan Duration { get; }

        /// <summary>
        ///     The ID of the event
        /// </summary>
        [JsonInclude]
        public string EventId { get; }

        /// <summary>
        ///     The start time of the event
        /// </summary>
        [JsonInclude]
        public DateTime Start { get; }

        /// <summary>
        ///     Whether or not the event has completed
        /// </summary>
        public bool Completed => Remaining <= TimeSpan.Zero;

        /// <summary>
        ///     The remaining time of the event
        /// </summary>
        public TimeSpan Remaining => Start + Duration - DateTime.UtcNow;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TimedEventCollection.Event" /> class
        /// </summary>
        /// <param name="eventId">
        ///     The unique id of the event
        /// </param>
        /// <param name="duration">
        ///     The duration of the event
        /// </param>
        /// <param name="start">
        ///     When the event started
        /// </param>
        /// <param name="autoConsume">
        ///     Whether or not the event should automatically be removed when it expires
        /// </param>
        [JsonConstructor]
        public Event(
            string eventId,
            TimeSpan duration,
            DateTime start,
            bool autoConsume)
        {
            EventId = eventId;
            Duration = duration;
            Start = start;
            AutoConsume = autoConsume;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TimedEventCollection.Event" /> class
        /// </summary>
        /// <param name="eventId">
        ///     The unique id of the event
        /// </param>
        /// <param name="duration">
        ///     The duration of the event
        /// </param>
        /// <param name="autoConsume">
        ///     Whether or not the event should automatically be removed when it expires
        /// </param>
        public Event(string eventId, TimeSpan duration, bool autoConsume = false)
        {
            EventId = eventId;
            Duration = duration;
            Start = DateTime.UtcNow;
            AutoConsume = autoConsume;
        }

        /// <inheritdoc />
        public bool Equals(Event? other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return StringComparer.OrdinalIgnoreCase.Equals(EventId, other.EventId);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is Event other && Equals(other));

        /// <inheritdoc />
        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(EventId);
    }
}