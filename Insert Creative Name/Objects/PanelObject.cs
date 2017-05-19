using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal abstract class PanelObject
    {
        internal byte Slot { get; set; }
        internal ushort Sprite { get; }
        internal string Name { get; }
        internal TimeSpan Cooldown { get; }
        internal DateTime LastUse { get; set; }

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
