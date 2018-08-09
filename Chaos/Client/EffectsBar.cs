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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    /// <summary>
    /// Object representing the spellbar.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class EffectsBar : IEnumerable<Effect>
    {
        internal readonly object Sync = new object();
        [JsonProperty]
        private List<Effect> Effects { get; set; }
        public int Count => Effects.Count;
        public IEnumerator<Effect> GetEnumerator() => Effects.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        [JsonConstructor]
        internal EffectsBar()
        {
            Effects = new List<Effect>();
        }

        internal bool TryAdd(Effect effect)
        {
            lock(Sync)
            {
                if (Effects.Contains(effect) || Effects.Any(e => effect.Icon == e.Icon))
                    return false;
                else
                {
                    Effects.Add(effect);
                    return true;
                }
            }
        }

        internal bool TryRemove(Effect effect)
        {
            lock(Sync)
            {
                return Effects.Remove(effect);
            }
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
