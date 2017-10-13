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
    internal abstract class VisibleObject : WorldObject
    {
        [JsonProperty]
        internal Point Point;
        [JsonProperty]
        internal ushort Sprite { get; }
        [JsonProperty]
        internal Map Map { get; set; }
        internal Location Location => new Location(Map.Id, Point);

        internal VisibleObject(string name, ushort sprite, Point point, Map map)
          : base(name)
        {
            Sprite = sprite;
            Point = point;
            Map = map;
        }

        internal bool WithinRange(Point p) => Point.Distance(p) < 12;
        internal bool WithinRange(VisibleObject v) => Point.Distance(v.Point) < 12;
    }
}
