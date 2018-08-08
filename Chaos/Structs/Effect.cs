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
        internal sbyte StrMod;
        internal sbyte IntMod;
        internal sbyte WisMod;
        internal sbyte ConMod;
        internal sbyte DexMod;
        internal int HPMod;
        internal int MPMod;
        internal Animation Animation;
        internal uint AnimationDelay;

        public static bool operator ==(Effect eff1, Effect eff2) => eff1.Equals(eff2);
        public static bool operator !=(Effect eff1, Effect eff2) => !eff1.Equals(eff2);

        internal Effect(Animation animation, uint animationDelay)
        {
            ID = Interlocked.Increment(ref ID_counter);
            StrMod = 0;
            IntMod = 0;
            WisMod = 0;
            ConMod = 0;
            DexMod = 0;
            HPMod = 0;
            MPMod = 0;
            Animation = animation;
            AnimationDelay = animationDelay;
        }

        [JsonConstructor]
        internal Effect(sbyte strMod, sbyte intMod, sbyte wisMod, sbyte conMod, sbyte dexMod, int hpMod, int mpMod, Animation animation, uint animationDelay)
        {
            ID = Interlocked.Increment(ref ID_counter);
            StrMod = strMod;
            IntMod = intMod;
            WisMod = wisMod;
            ConMod = conMod;
            DexMod = dexMod;
            HPMod = hpMod;
            MPMod = mpMod;
            Animation = animation;
            AnimationDelay = animationDelay;
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
