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
