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

namespace Chaos
{
    /// <summary>
    /// Represents an object that exists within the skill panel.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class Skill : PanelObject
    {
        internal bool IsBasic { get; }
        internal override bool CanUse => Elapsed.TotalMilliseconds >= CONSTANTS.GLOBAL_SKILL_COOLDOWN_MS && base.CanUse;

        /// <summary>
        /// Base constructor for an object that exists within the skill panel.
        /// </summary>
        internal Skill(ushort sprite, string name, TimeSpan baseCooldown, bool isBasic = false, Animation effectAnimation = new Animation(), TargetsType targetType = TargetsType.None, BodyAnimation bodyAnimation = 0, int baseDamage = 0)
            :this(0, sprite, name, baseCooldown, isBasic, effectAnimation, targetType, false, bodyAnimation, baseDamage)
        {
        }

        /// <summary>
        /// Master constructor for an object that exists within the skill panel.
        /// </summary>
        internal Skill(byte slot, ushort sprite, string name, TimeSpan baseCooldown, bool isBasic, Animation effectAnimation, TargetsType targetType, bool usersOnly, BodyAnimation bodyAnimation, int baseDamage)
            :base(slot, sprite, name, baseCooldown, effectAnimation, targetType, usersOnly, bodyAnimation, baseDamage)
        {
            IsBasic = isBasic;
        }

        /// <summary>
        /// Json constructor for a skill. Minimal information is serialized, as we retreive the skill from the creation engine, and apply persistent information to it.
        /// </summary>
        [JsonConstructor]
        private Skill(byte slot, string name, TimeSpan elapsed)
            :base(slot, name, elapsed)
        {
        }
    }
}
