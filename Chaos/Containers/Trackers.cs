using Chaos.Collections.Common;
using Chaos.Collections.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Containers;

public sealed class Trackers : IDeltaUpdatable
{
    public required CounterCollection Counters { get; init; }
    public required EnumCollection Enums { get; init; }
    public required FlagCollection Flags { get; init; }
    public required TimedEventCollection TimedEvents { get; init; }

    /// <inheritdoc />
    public void Update(TimeSpan delta) => TimedEvents.Update(delta);
}