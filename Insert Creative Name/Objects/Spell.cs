using System;

namespace Insert_Creative_Name.Objects
{
    [Serializable]
    internal sealed class Spell : PanelObject
    {
        internal byte Type { get; set; }
        internal string Prompt { get; set; }
        internal byte CastLines { get; set; }

        //for cooldowns
        internal bool CanUse => LastUse == DateTime.MinValue || Cooldown.Ticks == 0 || DateTime.UtcNow.Subtract(LastUse) > Cooldown;

        internal Spell(byte slot, string name, byte type, ushort sprite, string prompt, byte castLines, TimeSpan cooldown)
            :base(slot, sprite, name, cooldown)
        {
            Slot = slot;
            Type = type;
            Prompt = prompt;
            CastLines = castLines;
        }
    }
}
