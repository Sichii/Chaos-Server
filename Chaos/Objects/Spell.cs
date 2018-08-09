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
        internal override bool CanUse => DateTime.UtcNow.Subtract(LastUse).TotalMilliseconds >= CONSTANTS.GLOBAL_SPELL_COOLDOWN_MS && base.CanUse;

        /// <summary>
        /// Object representing a spell ability in your spell pane.
        /// </summary>
        internal Spell(ushort sprite, string name, SpellType type, string prompt, byte castLines, TimeSpan baseCooldown, Animation effectAnimation = new Animation(), TargetsType targetType = TargetsType.None, BodyAnimation bodyAnimation = BodyAnimation.None, int baseDamage = 0)
            :this(0, sprite, name, type, prompt, castLines, baseCooldown, effectAnimation, targetType, bodyAnimation, baseDamage)
        {
        }

        /// <summary>
        /// Master constructor for spell, do not use.
        /// </summary>
        [JsonConstructor]
        internal Spell(byte slot, ushort sprite, string name, SpellType type, string prompt, byte castlines, TimeSpan baseCooldown, Animation effectAnimation, TargetsType targetType, BodyAnimation bodyAnimation, int baseDamage)
            :base(slot, sprite, name, baseCooldown, effectAnimation, targetType, bodyAnimation, baseDamage)
        {
            SpellType = type;
            Prompt = prompt ?? "";
            CastLines = castlines;
        }
    }
}
