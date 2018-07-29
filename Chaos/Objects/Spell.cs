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
        internal SpellType Type { get; set; }
        [JsonProperty]
        internal string Prompt { get; set; }
        [JsonProperty]
        internal byte CastLines { get; set; }
        internal override bool CanUse => (LastUse == DateTime.MinValue || Cooldown.Ticks == 0 || DateTime.UtcNow.Subtract(LastUse) > Cooldown) && 
            DateTime.UtcNow.Subtract(LastUse).TotalMilliseconds >= CONSTANTS.GLOBAL_SPELL_COOLDOWN_MS;

        internal Spell(byte slot, ushort sprite, string name, SpellType type, string prompt, byte castLines, TimeSpan cooldown, Animation effectAnimation = new Animation(), BodyAnimation bodyAnimation = 0)
            :base(slot, sprite, name, cooldown, effectAnimation, bodyAnimation)
        {
            Type = type;
            Prompt = prompt;
            CastLines = castLines;
        }

        [JsonConstructor]
        internal Spell(byte slot, ushort sprite, string name, SpellType type, TimeSpan cooldown, string prompt, byte castlines, Animation effectAnimation, BodyAnimation bodyAnimation)
            :base(slot, sprite, name, cooldown, effectAnimation, bodyAnimation)
        {
            Type = type;
            Prompt = prompt;
            CastLines = castlines;
        }
    }
}
