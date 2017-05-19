using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal struct Spell
    {
        private static Dictionary<string, double> spellDurations = new Dictionary<string, double>()
        {
            { "beag fas nadur", 450.0 },
            { "fas nadur", 450.0 },
            { "mor fas nadur", 450.0 },
            { "ard fas nadur", 225.0 },
            { "beag cradh", 150.0 },
            { "cradh", 180.0 },
            { "mor cradh", 210.0 },
            { "ard cradh", 240.0 },
            { "Dark Seal", 150.0 },
            { "Darker Seal", 150.0 },
            { "Demise", 150.0 },
            { "beag naomh aite", 600.0 },
            { "naomh aite", 600.0 },
            { "mor naomh aite", 600.0 },
            { "ard naomh aite", 600.0 },
            { "mor dion", 20 },
            { "Iron Skin", 20 },
            { "mor dion comlha", 20 },
            { "dion", 10 },
            { "Wings of Protection", 13 },
            { "Asgall Faileas", 13 }
        };
        internal string Name { get; set; }
        internal byte Slot { get; set; }
        internal byte Type { get; set; }
        internal ushort Sprite { get; set; }
        internal string Prompt { get; set; }
        internal byte CastLines { get; set; }
        internal byte CurrentLevel { get; set; }
        internal byte MaximumLevel { get; set; }
        internal DateTime LastUse { get; set; }
        internal DateTime CooldownStart { get; set; }
        internal double CooldownLength { get; set; }

        //for cooldowns
        internal bool CanUse
        {
            get
            {
                //if hasnt been casted || no cooldown
                if (CooldownStart == DateTime.MinValue || CooldownLength == 0.0)
                    return true;
                //gets cooldown length and subtracts castlines(each is a second obviously) and a buffer time of 0.5seconds
                //this allows us to ignore ping and cast things the instant they come off cooldown
                double num = CooldownLength - CastLines + 0.5;

                //starts cooldown timers
                TimeSpan timeSpan1 = DateTime.UtcNow.Subtract(LastUse);
                TimeSpan timeSpan2 = DateTime.UtcNow.Subtract(CooldownStart);

                //minimum cooldown of 1.5seconds
                if (timeSpan1.TotalSeconds > 1.5)
                    //the elapsed time is longer than the cooldown: true... otherwise false
                    return timeSpan2.TotalSeconds > num;
                return false;
            }
        }

        internal Spell(byte slot, string name, byte type, ushort sprite, string prompt, byte castLines, byte currentLevel, byte maximumLevel, double cooldownLength)
        {
            Slot = slot;
            Name = name;
            Type = type;
            Sprite = sprite;
            Prompt = prompt;
            CastLines = castLines;
            CurrentLevel = currentLevel;
            MaximumLevel = maximumLevel;
            CooldownStart = DateTime.MinValue;
            CooldownLength = cooldownLength;
            LastUse = DateTime.MinValue;
        }

        internal static double GetSpellDuration(string name)
        {
            if (!spellDurations.ContainsKey(name))
                return 30.0;
            return spellDurations[name];
        }
    }
}
