using System;

namespace Chaos.Objects
{
    internal sealed class Item : PanelObject
    {
        internal byte Color { get; }
        internal int Count { get; set; }
        internal bool Stackable { get; }
        internal uint MaxDurability { get; }
        internal uint CurrentDurability { get; set; }

        internal Item(byte slot, ushort sprite, byte color, string name, int count, bool stackable, uint maximumDurability, uint currentDurability, TimeSpan cooldown)
            :base(slot, sprite, name, cooldown)
        {
            Color = color;
            Count = count;
            Stackable = stackable;
            MaxDurability = maximumDurability;
            CurrentDurability = currentDurability;
        }
    }
}
