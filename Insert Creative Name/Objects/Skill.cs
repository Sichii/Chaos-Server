using System;

namespace Insert_Creative_Name.Objects
{
    internal sealed class Skill : PanelObject
    {
        internal bool CanUse => LastUse == DateTime.MinValue || Cooldown.Ticks == 0 || DateTime.UtcNow.Subtract(LastUse) > Cooldown;

        internal Skill(byte slot, string name, ushort sprite, TimeSpan cooldown)
            :base(slot, sprite, name, cooldown)
        {
        }
    }
}
