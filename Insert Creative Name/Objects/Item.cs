using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal sealed class Item : PanelObject
    {
        internal byte Color { get; }
        internal int Count { get; set; }
        internal bool Stackable { get; }
        internal int MaxDurability { get; }
        internal int CurrentDurability { get; set; }

        internal Item(byte slot, ushort sprite, byte color, string name, int count, bool stackable, int maximumDurability, int currentDurability, TimeSpan cooldown)
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
