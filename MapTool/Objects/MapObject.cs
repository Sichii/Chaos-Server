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

namespace MapTool
{
    internal abstract class MapObject
    {
        internal Point Point => new Point(X, Y);
        internal Location Location => new Location(MapId, Point);
        protected ushort X;
        protected ushort Y;
        protected ushort MapId;

        protected internal MapObject(ushort mapId, ushort x, ushort y)
        {
            MapId = mapId;
            X = x;
            Y = y;
        }

        protected internal MapObject(Location location)
        {
            MapId = location.MapId;
            X = location.X;
            Y = location.Y;
        }

        protected internal MapObject(ushort mapId, Point point)
        {
            MapId = mapId;
            X = point.X;
            Y = point.Y;
        }
    }
}
