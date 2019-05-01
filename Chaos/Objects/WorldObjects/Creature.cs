// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Chaos
{
    /// <summary>
    /// Represents an object that can move, has hp, mp, and can be damaged.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class Creature : VisibleObject
    {
        [JsonProperty]
        protected Status Status { get; set; }
        [JsonProperty]
        internal EffectsBar EffectsBar { get; set; }
        [JsonProperty]
        internal Direction Direction { get; set; }
        [JsonProperty]
        internal CreatureType Type { get; }

        //Use for targeting only. !HasFlag(Status.Dead) otherwise
        internal bool IsAlive => CurrentHP > 0;
        internal abstract byte HealthPercent { get; }
        internal abstract uint MaximumHP { get; }
        internal abstract uint CurrentHP { get; set; }
        internal Dictionary<int, DateTime> AnimationHistory { get; set; }
        internal Dictionary<int, DateTime> WorldAnimationHistory { get; set; }
        internal Dictionary<int, DateTime> LastClicked { get; set; }
        internal bool ShouldDisplay(int id) => !LastClicked.TryGetValue(id, out DateTime lastClick) || DateTime.UtcNow.Subtract(lastClick).TotalMilliseconds > 500;

        /// <summary>
        /// Json & Master constructor for a creature.
        /// </summary>
        [JsonConstructor]
        protected Creature(string name, Location location, ushort sprite, CreatureType type, Direction direction = Direction.South, EffectsBar effectsBar = null)
            : base(name, location, sprite)
        {
            EffectsBar = effectsBar ?? new EffectsBar(null);
            Direction = direction;
            Type = type;

            AnimationHistory = new Dictionary<int, DateTime>();
            WorldAnimationHistory = new Dictionary<int, DateTime>();
            LastClicked = new Dictionary<int, DateTime>();
        }

        /// <summary>
        /// Checks if the creature has the given status flag.
        /// </summary> 
        internal bool HasFlag(Status flag) => Status.HasFlag(flag);

        /// <summary>
        /// Adds a status flag to the creature.
        /// </summary>
        internal void AddFlag(Status flag) => Status |= flag;

        /// <summary>
        /// Removes a status flag from the creature.
        /// </summary>
        internal void RemoveFlag(Status flag) => Status &= ~flag;
    }
}
