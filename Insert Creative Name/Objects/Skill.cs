using System;

namespace Insert_Creative_Name.Objects
{
    [Serializable]
    internal sealed class Skill : PanelObject
    {
        internal Skill(byte slot, string name, ushort sprite, TimeSpan cooldown)
            :base(slot, sprite, name, cooldown)
        {
        }
    }
}
