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
    /// Represents an object that exists within the spell panel.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class Spell : PanelObject
    {
        internal SpellType SpellType { get; set; }
        internal string Prompt { get; set; }
        internal byte CastLines { get; set; }
        internal override bool CanUse => Elapsed.TotalMilliseconds >= CONSTANTS.GLOBAL_SPELL_COOLDOWN_MS && base.CanUse;

        /// <summary>
        /// Base constructor for an object that exists within the spell panel.
        /// </summary>
        internal Spell(ushort sprite, string name, SpellType type, string prompt, byte castLines, TimeSpan baseCooldown, Animation effectAnimation = new Animation(), 
            TargetsType targetType = TargetsType.None, bool usersOnly = false, BodyAnimation bodyAnimation = BodyAnimation.None, int baseDamage = 0, Effect effect = default)
            :this(0, sprite, name, type, prompt, castLines, baseCooldown, effectAnimation, targetType, usersOnly, bodyAnimation, baseDamage, effect)
        {
        }

        /// <summary>
        /// Master constructor for an object that exists within the spell panel.
        /// </summary>
        internal Spell(byte slot, ushort sprite, string name, SpellType type, string prompt, byte castlines, TimeSpan baseCooldown, Animation effectAnimation, 
            TargetsType targetType, bool usersOnly, BodyAnimation bodyAnimation, int baseDamage, Effect effect)
            :base(slot, sprite, name, baseCooldown, effectAnimation, targetType, usersOnly, bodyAnimation, baseDamage, effect)
        {
            SpellType = type;
            Prompt = prompt ?? "";
            CastLines = castlines;
        }

        /// <summary>
        /// Json constructor for a spell. Minimal information is serialized, as we retreive the spell from the creation engine, and apply persistent information to it.
        /// </summary>
        [JsonConstructor]
        private Spell(byte slot, string name, TimeSpan elapsed)
            :base(slot, name, elapsed)
        {
        }
    }
}
