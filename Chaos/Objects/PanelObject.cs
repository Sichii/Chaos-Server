using System;
using Newtonsoft.Json;

namespace Chaos.Objects
{
    internal abstract class PanelObject
    {
        [JsonProperty]
        internal byte Slot { get; set; }
        [JsonProperty]
        internal ushort Sprite { get; }
        [JsonProperty]
        internal string Name { get; }
        [JsonProperty]
        internal TimeSpan Cooldown { get; }
        [JsonProperty]
        internal DateTime LastUse { get; set; }
        internal bool CanUse => LastUse == DateTime.MinValue || Cooldown.Ticks == 0 || DateTime.UtcNow.Subtract(LastUse) > Cooldown;

        internal PanelObject(byte slot, ushort sprite, string name, TimeSpan cooldown)
        {
            Slot = slot;
            Sprite = sprite;
            Name = name;
            Cooldown = cooldown;
            LastUse = DateTime.MinValue;
        }

    }
}
