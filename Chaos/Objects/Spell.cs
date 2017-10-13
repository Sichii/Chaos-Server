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
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Spell : PanelObject
    {
        [JsonProperty]
        internal SpellType Type { get; set; }
        [JsonProperty]
        internal string Prompt { get; set; }
        [JsonProperty]
        internal byte CastLines { get; set; }

        internal Spell(byte slot, string name, SpellType type, ushort sprite, string prompt, byte castLines, TimeSpan cooldown)
            :base(slot, sprite, name, cooldown)
        {
            Type = type;
            Prompt = prompt;
            CastLines = castLines;
        }

        [JsonConstructor]
        internal Spell(byte slot, ushort sprite, string name, TimeSpan cooldown, SpellType type, string prompt, byte castlines)
            :base(slot, sprite, name, cooldown)
        {
            Type = type;
            Prompt = prompt;
            CastLines = castlines;
        }
    }
}
