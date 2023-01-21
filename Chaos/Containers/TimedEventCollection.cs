using Chaos.Common.Synchronization;
using Chaos.Data;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Containers;

public sealed class TimedEventCollection : IEnumerable<TimedEvent>, IDeltaUpdatable
{
    private readonly Dictionary<TimedEvent.TimedEventId, HashSet<TimedEvent>> Events;
    private readonly IIntervalTimer Interval;
    private readonly AutoReleasingMonitor Sync;

    public TimedEventCollection(IEnumerable<TimedEvent>? events = null)
    {
        events ??= Array.Empty<TimedEvent>();
        Events = new Dictionary<TimedEvent.TimedEventId, HashSet<TimedEvent>>();
        Sync = new AutoReleasingMonitor();
        Interval = new IntervalTimer(TimeSpan.FromSeconds(1));

        foreach (var timedEvent in events)
            InnerAddEvent(timedEvent);
    }

    public void AddEvent(TimedEvent.TimedEventId eventId, TimeSpan duration, bool autoConsume = false)
    {
        using var sync = Sync.Enter();

        var timedEvent = new TimedEvent(eventId, duration, autoConsume);

        InnerAddEvent(timedEvent);
    }

    /// <inheritdoc />
    public IEnumerator<TimedEvent> GetEnumerator()
    {
        HashSet<TimedEvent> snapShot;

        using (Sync.Enter())
            snapShot = Events.Values.SelectMany(_ => _).ToHashSet();

        return snapShot.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void InnerAddEvent(TimedEvent timedEvent)
    {
        if (!Events.TryGetValue(timedEvent.EventId, out var eventList))
        {
            eventList = new HashSet<TimedEvent>();
            Events.Add(timedEvent.EventId, eventList);
        }

        eventList.Add(timedEvent);
    }

    public bool TryConsumeEvent(TimedEvent.TimedEventId eventId, [MaybeNullWhen(false)] out TimedEvent timedEvent)
    {
        using var sync = Sync.Enter();

        timedEvent = null;

        if (!Events.TryGetValue(eventId, out var eventList))
            return false;

        timedEvent = eventList.FirstOrDefault(e => e.Completed);

        if (timedEvent != null)
            eventList.Remove(timedEvent);

        return timedEvent != null;
    }

    public bool TryGetNearestToCompletion(TimedEvent.TimedEventId eventId, [MaybeNullWhen(false)] out TimedEvent timedEvent)
    {
        timedEvent = null;

        if (!Events.TryGetValue(eventId, out var events) || !events.Any())
            return false;

        timedEvent = events.MinBy(e => e.Remaining);

        return timedEvent != null;
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        Interval.Update(delta);

        if (Interval.IntervalElapsed)
        {
            using var sync = Sync.Enter();

            foreach (var kvp in Events)
            {
                var timedEvents = kvp.Value.Where(timedEvent => timedEvent.AutoConsume)
                                     .ToList();

                foreach (var timer in timedEvents)
                    if (timer.Completed)
                        kvp.Value.Remove(timer);
            }
        }
    }
}