// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using Newtonsoft.Json;
using System;
using System.Threading;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal struct Effect
    {
        [JsonIgnore]
        private static int ID_counter;
        [JsonIgnore]
        internal int ID;
        [JsonIgnore]
        internal DateTime StartTime;

        internal ushort Icon;
        internal sbyte StrMod;
        internal sbyte IntMod;
        internal sbyte WisMod;
        internal sbyte ConMod;
        internal sbyte DexMod;
        internal int MaxHPMod;
        internal int MaxMPMod;
        internal int CurrentHPMod;
        internal int CurrentMPMod;
        internal Animation Animation;
        internal uint AnimationDelay;
        internal TimeSpan Duration;

        public static bool operator ==(Effect eff1, Effect eff2) => eff1.Equals(eff2);
        public static bool operator !=(Effect eff1, Effect eff2) => !eff1.Equals(eff2);

        internal Effect(Animation animation, uint animationDelay, TimeSpan duration)
            :this(ushort.MaxValue, 0, 0, 0, 0, 0, 0, 0, 0, 0, animation, animationDelay, duration)
        {
        }

        [JsonConstructor]
        internal Effect(ushort icon, sbyte strMod, sbyte intMod, sbyte wisMod, sbyte conMod, sbyte dexMod, int maxHPMod, int maxMPMod, int currentHPMod, int currentMPMod, Animation animation, uint animationDelay, TimeSpan duration)
        {
            StartTime = DateTime.UtcNow;
            ID = Interlocked.Increment(ref ID_counter);

            Icon = icon;
            StrMod = strMod;
            IntMod = intMod;
            WisMod = wisMod;
            ConMod = conMod;
            DexMod = dexMod;
            MaxHPMod = maxHPMod;
            MaxMPMod = maxMPMod;
            CurrentHPMod = currentHPMod;
            CurrentMPMod = currentMPMod;
            Animation = animation;
            AnimationDelay = animationDelay;
            Duration = duration;
        }

        internal int RemainingDurationMS()
        {
            TimeSpan elapsed = DateTime.UtcNow.Subtract(StartTime);

            if (elapsed > Duration)
                return 0;
            else
                return (int)Duration.Subtract(elapsed).TotalMilliseconds;
        }

        internal EffectsBarColor Color()
        {
            int ms = RemainingDurationMS();

            if (ms >= 60000) //1min
                return EffectsBarColor.White;
            else if (ms >= 45000)
                return EffectsBarColor.Red;
            else if (ms >= 30000)
                return EffectsBarColor.Orange;
            else if (ms >= 15000)
                return EffectsBarColor.Yellow;
            else if (ms >= 5000)
                return EffectsBarColor.Green;
            else if (ms > 0)
                return EffectsBarColor.Blue;
            else
                return EffectsBarColor.None;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Effect))
                return false;

            Effect eff = (Effect)obj;

            return GetHashCode() == eff.GetHashCode();
        }
        public override int GetHashCode() => ID;
    }
}
