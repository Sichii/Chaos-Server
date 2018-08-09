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
using System.Collections.Generic;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class Creature : VisibleObject
    {
        [JsonProperty]
        internal EffectsBar EffectsBar { get; set; }
        [JsonProperty]
        internal Direction Direction { get; set; }
        [JsonProperty]
        internal CreatureType Type { get; }
        internal bool IsAlive => CurrentHP > 0;
        internal abstract byte HealthPercent { get; }
        internal abstract uint MaximumHP { get; }
        internal abstract uint CurrentHP { get; set; }
        internal Dictionary<(int, Point, ushort), DateTime> AnimationHistory { get; set; }


        [JsonConstructor]
        internal Creature(string name, ushort sprite, CreatureType type, Point point, Map map, Direction direction = Direction.South, EffectsBar effectsBar = null)
            : base(name, sprite, point, map)
        {
            EffectsBar = effectsBar ?? new EffectsBar();
            Direction = direction;
            Type = type;

            AnimationHistory = new Dictionary<(int, Point, ushort), DateTime>();
        }

    }
}
