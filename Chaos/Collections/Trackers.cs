#region
using System.Net;
using Chaos.Collections.Common;
using Chaos.Collections.Specialized;
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
    /// <inheritdoc />
    public void Update(TimeSpan delta) => TimedEvents.Update(delta);

    #region Persistent
    /// <summary>
    ///     A collection of integral counters organized by key
    /// </summary>
    /// <remarks>
    ///     IS PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public CounterCollection Counters { get; init; } = new();

    /// <summary>
    ///     A collection of enums organized by type
    /// </summary>
    /// <remarks>
    ///     IS PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public EnumCollection Enums { get; init; } = new();

    /// <summary>
    ///     A collection of enum flags organized by type
    /// </summary>
    /// <remarks>
    ///     IS PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public FlagCollection Flags { get; init; } = new();

    /// <summary>
    ///     A collection of timed events organized by key
    /// </summary>
    /// <remarks>
    ///     IS PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public TimedEventCollection TimedEvents { get; init; } = new();
    #endregion

    #region Not Persistent
    /// <summary>
    ///     The creature that last damaged this entity
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public Creature? LastDamagedBy { get; set; }

    /// <summary>
    ///     The instance id of the last map this entity was on (excluding the current map)
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public string? LastMapInstanceId { get; set; }

    /// <summary>
    ///     The last location this entity was at (excluding the current location)
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public Location? LastPosition { get; set; }

    /// <summary>
    ///     The time the entity last used a skill
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastSkillUse { get; set; }

    /// <summary>
    ///     The time the entity last used a spell
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastSpellUse { get; set; }

    /// <summary>
    ///     The time the entity last talked
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastTalk { get; set; }

    /// <summary>
    ///     The time the entity last turned
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastTurn { get; set; }

    /// <summary>
    ///     The skill that was last used by this entity
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public Skill? LastUsedSkill { get; set; }

    /// <summary>
    ///     The spell that was last used by this entity
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public Spell? LastUsedSpell { get; set; }

    /// <summary>
    ///     The time the entity last walked
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastWalk { get; set; }
    #endregion
}

/// <summary>
///     An object used to track various historical data points about an aisling
/// </summary>
public sealed class AislingTrackers : Trackers
{
    #region Persistent
    /// <summary>
    ///     The last 10 ip addresses associated with this aisling
    /// </summary>
    /// <remarks>
    ///     IS PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public FixedSet<IPAddress> AssociatedIpAddresses { get; set; } = new(
        10,
        comparer: EqualityComparer<IPAddress>.Create((x, y) => x?.Equals(y) ?? false, ip => ip.GetHashCode()));

    /// <summary>
    ///     The last IpAddress used by this aisling
    /// </summary>
    /// <remarks>
    ///     IS PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public IPAddress? LastIpAddress { get; set; }

    /// <summary>
    ///     The time the aisling last logged in
    /// </summary>
    /// <remarks>
    ///     IS PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    ///     The time the aisling last logged out
    /// </summary>
    /// <remarks>
    ///     IS PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastLogout { get; set; }
    #endregion

    #region Not Persistent
    /// <summary>
    ///     The time the aisling last equipped an item
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastEquip { get; set; }

    /// <summary>
    ///     The time the aisling last manually acted
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastManualAction { get; set; }

    /// <summary>
    ///     The time the aisling last received an orange bar message
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastOrangeBarMessage { get; set; }

    /// <summary>
    ///     The time the aisling last cleared the orange bar of messages
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastOrangeBarMessageClear { get; set; }

    /// <summary>
    ///     The time the aisling last refreshed
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastRefresh { get; set; }

    /// <summary>
    ///     The time the aisling last unequipped an item
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastUnequip { get; set; }

    /// <summary>
    ///     The time the aisling last equipped or unequipped an item
    /// </summary>
    /// <remarks>
    ///     NOT PERSISTENT / SERIALIZED TO FILE
    /// </remarks>
    public DateTime? LastEquipOrUnequip
    {
        get
        {
            var le = LastEquip ?? DateTime.MinValue;
            var lue = LastUnequip ?? DateTime.MinValue;

            return le > lue ? le : lue;
        }
    }
    #endregion
}