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
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class Skill : PanelObject
    {
        [JsonProperty]
        internal bool IsBasic { get; }

        internal override bool CanUse => DateTime.UtcNow.Subtract(LastUse).TotalMilliseconds >= CONSTANTS.GLOBAL_SKILL_COOLDOWN_MS;

        /// <summary>
        /// Object representing a skill in your skill pane.
        /// </summary>
        internal Skill(ushort sprite, string name, TimeSpan cooldown, bool isBasic = false, Animation effectAnimation = new Animation(), TargetsType targetType = TargetsType.None, BodyAnimation bodyAnimation = 0, int baseDamage = 0)
            :this(0, sprite, name, cooldown, isBasic, effectAnimation, targetType, bodyAnimation, baseDamage)
        {
        }

        /// <summary>
        /// Master constructor for skills, do not use.
        /// </summary>
        [JsonConstructor]
        internal Skill(byte slot, ushort sprite, string name, TimeSpan cooldown, bool isBasic, Animation effectAnimation, TargetsType targetType, BodyAnimation bodyAnimation, int baseDamage)
            :base(slot, sprite, name, cooldown, effectAnimation, targetType, bodyAnimation, baseDamage)
        {
            IsBasic = isBasic;
        }
    }
}
