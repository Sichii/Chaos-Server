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
    internal sealed class Spell : PanelObject
    {
        [JsonProperty]
        internal SpellType SpellType { get; set; }
        [JsonProperty]
        internal string Prompt { get; set; }
        [JsonProperty]
        internal byte CastLines { get; set; }
        internal override bool CanUse => (LastUse == DateTime.MinValue || Cooldown.Ticks == 0 || DateTime.UtcNow.Subtract(LastUse) > Cooldown) && 
            DateTime.UtcNow.Subtract(LastUse).TotalMilliseconds >= CONSTANTS.GLOBAL_SPELL_COOLDOWN_MS;

        /// <summary>
        /// Object representing a spell ability in your spell pane.
        /// </summary>
        internal Spell(ushort sprite, string name, SpellType type, string prompt, byte castLines, TimeSpan cooldown, Animation effectAnimation = new Animation(), TargetsType targetType = TargetsType.None, BodyAnimation bodyAnimation = BodyAnimation.None, int baseDamage = 0)
            :this(0, sprite, name, type, prompt, castLines, cooldown, effectAnimation, targetType, bodyAnimation, baseDamage)
        {
        }

        /// <summary>
        /// Master constructor for spell, do not use.
        /// </summary>
        [JsonConstructor]
        internal Spell(byte slot, ushort sprite, string name, SpellType type, string prompt, byte castlines, TimeSpan cooldown, Animation effectAnimation, TargetsType targetType, BodyAnimation bodyAnimation, int baseDamage)
            :base(slot, sprite, name, cooldown, effectAnimation, targetType, bodyAnimation, baseDamage)
        {
            SpellType = type;
            Prompt = prompt;
            CastLines = castlines;
        }
    }
}
