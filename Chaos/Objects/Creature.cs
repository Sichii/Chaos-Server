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

namespace Chaos
{
    internal abstract class Creature : VisibleObject
    {
        [JsonProperty]
        internal Direction Direction { get; set; }
        [JsonProperty]
        internal CreatureType Type { get; }
        internal abstract byte HealthPercent { get; }
        internal abstract uint MaximumHP { get; }
        internal abstract uint CurrentHP { get; set; }


        internal Creature(string name, ushort sprite, CreatureType type, Point point, Map map, Direction direction = Direction.South)
            : base(name, sprite, point, map)
        {
            Direction = direction;
            Type = type;
        }
    }
}
