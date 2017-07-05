using System;

namespace Chaos.Objects
{
    internal sealed class Skill : PanelObject
    {
        internal Skill(byte slot, string name, ushort sprite, TimeSpan cooldown)
            :base(slot, sprite, name, cooldown)
        {
        }
    }
}
