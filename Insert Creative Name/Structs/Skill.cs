using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal struct Skill
    {
        internal string Name { get; set; }
        internal byte Slot { get; set; }
        internal ushort Sprite { get; set; }
        internal byte CurrentLevel { get; set; }
        internal byte MaximumLevel { get; set; }
        internal DateTime LastUse { get; set; }
        internal DateTime CooldownStart { get; set; }
        internal double CooldownLength { get; set; }

        internal bool CanUse
        {
            get
            {
                if (CooldownStart == DateTime.MinValue || CooldownLength == 0.0)
                    return true;
                double num = Math.Max(1.0, CooldownLength);
                TimeSpan timeSpan1 = DateTime.UtcNow.Subtract(LastUse);
                TimeSpan timeSpan2 = DateTime.UtcNow.Subtract(CooldownStart);
                if (timeSpan1.TotalSeconds > 0.5)
                    return timeSpan2.TotalSeconds > num;
                return false;
            }
        }

        internal Skill(byte slot, string name, ushort sprite, byte currentLevel, byte maximumLevel, double cooldownLength)
        {
            Slot = slot;
            Name = name;
            Sprite = sprite;
            CurrentLevel = currentLevel;
            MaximumLevel = maximumLevel;
            LastUse = DateTime.MinValue;
            CooldownStart = DateTime.MinValue;
            CooldownLength = cooldownLength;
        }
    }
}
