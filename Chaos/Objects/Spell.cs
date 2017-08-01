using Newtonsoft.Json;
using System;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Spell : PanelObject
    {
        [JsonProperty]
        internal byte Type { get; set; }
        [JsonProperty]
        internal string Prompt { get; set; }
        [JsonProperty]
        internal byte CastLines { get; set; }

        internal Spell(byte slot, string name, byte type, ushort sprite, string prompt, byte castLines, TimeSpan cooldown)
            :base(slot, sprite, name, cooldown)
        {
            Type = type;
            Prompt = prompt;
            CastLines = castLines;
        }

        [JsonConstructor]
        internal Spell(byte slot, ushort sprite, string name, TimeSpan cooldown, byte type, string prompt, byte castlines)
            :base(slot, sprite, name, cooldown)
        {
            Type = type;
            Prompt = prompt;
            CastLines = castlines;
        }
    }
}
