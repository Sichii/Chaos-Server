using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal abstract class MapObject
    {
        protected Point Point => new Point(X, Y);
        protected Location Location => new Location(MapId, Point);
        protected short X;
        protected short Y;
        protected ushort MapId;

        protected internal MapObject(ushort mapId, short x, short y)
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
