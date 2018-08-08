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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos
{
    /// <summary>
    /// Object representing the spellbar.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class EffectsBar
    {
        internal readonly object Sync = new object();
        private Dictionary<Effect, DateTime> Effects { get; set; }

        [JsonConstructor]
        internal EffectsBar()
        {
            Effects = new Dictionary<Effect, DateTime>();
        }

        internal bool TryAdd(Effect effect)
        {
            lock(Sync)
            {
                if (Effects.ContainsKey(effect))
                    return false;
                else
                {
                    Effects.Add(effect, DateTime.UtcNow);
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

        internal bool TryGet(int id, out Effect effect)
        {
            effect = Effects.Keys.FirstOrDefault(e => e.ID == id);
            return effect == default(Effect);
        }
    }
}
