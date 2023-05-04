using Chaos.Collections.Common;
using Chaos.Collections.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Collections;

public sealed class Trackers : IDeltaUpdatable
{
    public required CounterCollection Counters { get; init; }
    public required EnumCollection Enums { get; init; }
    public required FlagCollection Flags { get; init; }
    public DateTime? LastCast { get; set; }

    public DateTime? LastEquip { get; set; }
    public DateTime? LastManualAction { get; set; }
    public DateTime? LastRefresh { get; set; }
    public DateTime? LastSkill { get; set; }
    public DateTime? LastUnequip { get; set; }
    public required TimedEventCollection TimedEvents { get; init; }

    public DateTime? LastEquipOrUnequip => LastEquip > LastUnequip ? LastEquip : LastUnequip;

    /// <inheritdoc />
    public void Update(TimeSpan delta) => TimedEvents.Update(delta);
}