using System;

namespace Chaos.Objects
{
    [Serializable]
    internal abstract class PanelObject
    {
        internal byte Slot { get; set; }
        internal ushort Sprite { get; }
        internal string Name { get; }
        internal TimeSpan Cooldown { get; }
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
