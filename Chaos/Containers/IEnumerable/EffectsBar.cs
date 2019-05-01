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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class EffectsBar : IEnumerable<Effect>
    {
        private readonly object Sync = new object();

        [JsonProperty]
        private readonly List<Effect> Effects;
    
        internal int Count => Effects.Count;

        /// <summary>
        /// Json & Master constructor for an enumerable object of Effect. Represents a user's spell bar.
        /// </summary>
        [JsonConstructor]
        internal EffectsBar(List<Effect> effects)
        {
            Effects = effects ?? new List<Effect>();
        }

        public IEnumerator<Effect> GetEnumerator()
        {
            lock (Sync)
                using (IEnumerator<Effect> safeEnum = Effects.GetEnumerator())
                    while (safeEnum.MoveNext())
                        yield return safeEnum.Current;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Attempts to synchronously add an effect to the spell bar.
        /// </summary>
        /// <param name="effect">The effect to add.</param>
        internal bool TryAdd(Effect effect)
        {
            lock(Sync)
            {
                if (Effects.Contains(effect) || Effects.Any(e => effect.Sprite == e.Sprite))
                    return false;
                else
                {
                    Effects.Add(effect);
                    return true;
                }
            }
        }

        /// <summary>
        /// Attempts to synchronously remove an effect from the spell bar.
        /// </summary>
        /// <param name="effect">The effect to remove.</param>
        internal bool TryRemove(Effect effect)
        {
            lock(Sync)
                return Effects.Remove(effect);
        }

        /// <summary>
        /// Synchronously clears the effects bar of all effects.
        /// </summary>
        internal void Clear()
        {
            lock (Sync)
                Effects.ForEach(eff => eff.EndEffect());
        }

        internal int StrModSum => Effects.Sum(e => e.StrMod);
        internal int IntModSum => Effects.Sum(e => e.IntMod);
        internal int WisModSum => Effects.Sum(e => e.WisMod);
        internal int ConModSum => Effects.Sum(e => e.ConMod);
        internal int DexModSum => Effects.Sum(e => e.DexMod);
        internal int MaxHPModSum => Effects.Sum(e => e.MaxHPMod);
        internal int MaxMPModSum => Effects.Sum(e => e.MaxMPMod);
        internal int CurrentHPModSum =>Effects.Sum(e => e.CurrentHPMod);
        internal int CurrentMPModSum => Effects.Sum(e => e.CurrentMPMod);
    }
}
