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
        internal SkillType Type { get; }
        [JsonProperty]
        internal bool IsBasic { get; }

        internal override bool CanUse => DateTime.UtcNow.Subtract(LastUse).TotalMilliseconds >= CONSTANTS.GLOBAL_SKILL_COOLDOWN_MS;

        internal Skill(byte slot, ushort sprite, string name, SkillType type, TimeSpan cooldown, Animation effectAnimation = new Animation(), BodyAnimation bodyAnimation = 0)
            :base(slot, sprite, name, cooldown, effectAnimation, bodyAnimation)
        {
            Type = type;
        }

        [JsonConstructor]
        internal Skill(byte slot, ushort sprite, string name, SkillType type, TimeSpan cooldown, bool isBasic, Animation effectAnimation, BodyAnimation bodyAnimation)
            :base(slot, sprite, name, cooldown, effectAnimation, bodyAnimation)
        {
            Type = type;
            IsBasic = isBasic;
        }
    }
}
