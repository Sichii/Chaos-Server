// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provIDed that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using Newtonsoft.Json;
using System;

namespace Chaos
{
    /// <summary>
    /// Represents an object on the map.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class MapObject : WorldObject, IEquatable<MapObject>
    {
        internal readonly Type TypeRef = typeof(MapObject);

        [JsonProperty]
        public Location Location { get; set; }

        internal Map Map => Game.World.Maps.TryGetValue(Location.MapID, out Map map) ? map : null;
        public Point Point => Location.Point;

        /// <summary>
        /// Master constructor for an object representing something that exists on the map.
        /// </summary>
        protected internal MapObject(string name, Location location)
            :base(name)
        {
            Location = location;
        }

        public bool Equals(MapObject other) => !(other is null) && GetHashCode() == other.GetHashCode();
        public override int GetHashCode() => (ID << 16) + Point.GetHashCode();

        public override string ToString() => Location.ToString();
    }
}
