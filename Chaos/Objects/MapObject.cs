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

namespace Chaos
{
    /// <summary>
    /// Represents an object on the map.
    /// </summary>
    public abstract class MapObject
    {
        public Point Point => new Point(X, Y);
        internal Location Location => new Location(MapId, Point);
        public ushort X;
        public ushort Y;
        public ushort MapId;

        /// <summary>
        /// Optional constructor that takes a location.
        /// </summary>
        protected internal MapObject(Location location)
            :this(location.MapId, location.Point)
        {
        }

        /// <summary>
        /// Optional constructor that takes a point.
        /// </summary>
        protected internal MapObject(ushort mapId, Point point)
            :this(mapId, point.X, point.Y)
        {
        }

        /// <summary>
        /// Master constructor for an object representing something that exists on the map.
        /// </summary>
        protected internal MapObject(ushort mapId, ushort x, ushort y)
        {
            MapId = mapId;
            X = x;
            Y = y;
        }
    }
}
