using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal sealed class Item
    {
        internal byte Slot { get; set; }
        internal ushort Sprite { get; set; }
        internal string Name { get; set; }
        internal byte Color { get; set; }
        internal int Count { get; set; }
        internal int MaxDurability { get; set; }
        internal int CurrentDurability { get; set; }
        internal DateTime LastUse { get; set; }

        internal Item(byte slot, ushort sprite, byte color, string name, int count, int maximumDurability, int currentDurability)
        {
            Slot = slot;
            Sprite = sprite;
            Color = color;
            Name = name;
            Count = count;
            MaxDurability = maximumDurability;
            CurrentDurability = currentDurability;
            LastUse = DateTime.MinValue;
        }
    }
}
