#region
using Chaos.Collections.Common;
using Chaos.Collections.Time;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Collections;

/// <summary>
///     An object used to track various historical data points about an entity
/// </summary>
public class Trackers : IDeltaUpdatable
{
    /// <summary>
    ///     A collection of integral counters organized by key
    /// </summary>
    public CounterCollection Counters { get; init; } = new();

    /// <summary>
    ///     A collection of enums organized by type
    /// </summary>
    public EnumCollection Enums { get; init; } = new();

    /// <summary>
    ///     A collection of enum flags organized by type
    /// </summary>
    public FlagCollection Flags { get; init; } = new();

    /// <summary>
    ///     The creature that last damaged this entity
    /// </summary>
    public Creature? LastDamagedBy { get; set; }

    /// <summary>
    ///     The instance id of the last map this entity was on (excluding the current map)
    /// </summary>
    public string? LastMapInstanceId { get; set; }

    /// <summary>
    ///     The last location this entity was at (excluding the current location)
    /// </summary>
    public Location? LastPosition { get; set; }

    /// <summary>
    ///     The time the entity last used a skill
    /// </summary>
    public DateTime? LastSkillUse { get; set; }

    /// <summary>
    ///     The time the entity last used a spell
    /// </summary>
    public DateTime? LastSpellUse { get; set; }

    /// <summary>
    ///     The time the entity last talked
    /// </summary>
    public DateTime? LastTalk { get; set; }

    /// <summary>
    ///     The time the entity last turned
    /// </summary>
    public DateTime? LastTurn { get; set; }

    /// <summary>
    ///     The skill that was last used by this entity
    /// </summary>
    public Skill? LastUsedSkill { get; set; }

    /// <summary>
    ///     The spell that was last used by this entity
    /// </summary>
    public Spell? LastUsedSpell { get; set; }

    /// <summary>
    ///     The time the entity last walked
    /// </summary>
    public DateTime? LastWalk { get; set; }

    /// <summary>
    ///     A collection of timed events organized by key
    /// </summary>
    public TimedEventCollection TimedEvents { get; init; } = new();

    /// <inheritdoc />
    public void Update(TimeSpan delta) => TimedEvents.Update(delta);
}

/// <summary>
///     An object used to track various historical data points about an aisling
/// </summary>
public sealed class AislingTrackers : Trackers
{
    /// <summary>
    ///     The time the aisling last equipped an item
    /// </summary>
    public DateTime? LastEquip { get; set; }

    /// <summary>
    ///     The time the aisling last manually acted
    /// </summary>
    public DateTime? LastManualAction { get; set; }

    /// <summary>
    ///     The time the aisling last received an orange bar message
    /// </summary>
    public DateTime? LastOrangeBarMessage { get; set; }

    /// <summary>
    ///     The time the aisling last cleared the orange bar of messages
    /// </summary>
    public DateTime? LastOrangeBarMessageClear { get; set; }

    /// <summary>
    ///     The time the aisling last refreshed
    /// </summary>
    public DateTime? LastRefresh { get; set; }

    /// <summary>
    ///     The time the aisling last unequipped an item
    /// </summary>
    public DateTime? LastUnequip { get; set; }

    /// <summary>
    ///     The time the aisling last equipped or unequipped an item
    /// </summary>
    public DateTime? LastEquipOrUnequip
    {
        get
        {
            var le = LastEquip ?? DateTime.MinValue;
            var lue = LastUnequip ?? DateTime.MinValue;

            return le > lue ? le : lue;
        }
    }
}