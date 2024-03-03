using Chaos.Collections.Common;
using Chaos.Collections.Time;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Collections;

public class Trackers : IDeltaUpdatable
{
    public CounterCollection Counters { get; init; } = new();
    public EnumCollection Enums { get; init; } = new();
    public FlagCollection Flags { get; init; } = new();
    public Creature? LastDamagedBy { get; set; }
    public string? LastMapInstanceId { get; set; }
    public Location? LastPosition { get; set; }
    public DateTime? LastSkillUse { get; set; }
    public DateTime? LastSpellUse { get; set; }
    public DateTime? LastTalk { get; set; }
    public DateTime? LastTurn { get; set; }
    public Skill? LastUsedSkill { get; set; }
    public Spell? LastUsedSpell { get; set; }
    public DateTime? LastWalk { get; set; }
    public TimedEventCollection TimedEvents { get; init; } = new();

    /// <inheritdoc />
    public void Update(TimeSpan delta) => TimedEvents.Update(delta);
}

public sealed class AislingTrackers : Trackers
{
    public DateTime? LastEquip { get; set; }
    public DateTime? LastManualAction { get; set; }
    public DateTime? LastOrangeBarMessage { get; set; }
    public DateTime? LastOrangeBarMessageClear { get; set; }
    public DateTime? LastRefresh { get; set; }
    public DateTime? LastUnequip { get; set; }
    public DateTime? LastEquipOrUnequip => LastEquip > LastUnequip ? LastEquip : LastUnequip;
}