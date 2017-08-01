using Newtonsoft.Json;
using System;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Skill : PanelObject
    {
        [JsonProperty]
        internal bool IsBasic { get; }
        [JsonProperty]
        internal byte Animation { get; }

        internal Skill(byte slot, string name, ushort sprite, TimeSpan cooldown)
            :base(slot, sprite, name, cooldown)
        {
        }

        [JsonConstructor]
        internal Skill(byte slot, ushort sprite, string name, TimeSpan cooldown, bool isBasic, byte animation)
            :base(slot, sprite, name, cooldown)
        {
            IsBasic = isBasic;
            Animation = animation;
        }
        
    }
}
