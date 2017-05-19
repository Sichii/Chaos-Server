using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal class Door : MapObject
    {
        internal Location Location => MSourceLocation;
        internal Point Point => MSourcePoint;
        internal short X => MSourceX;
        internal short Y => MSourceY;
        internal ushort MapId => MSourceMapId;
        internal bool RecentlyClicked => DateTime.UtcNow.Subtract(LastClick).TotalSeconds < 1.5;
        internal bool Closed { get; set; }
        internal DateTime LastClick { get; set; }

        internal Door(Location location, bool closed)
        {
            MSourceMapId = location.MapId;
            MSourceX = location.Point.X;
            MSourceY = location.Point.Y;
            Closed = closed;
            LastClick = DateTime.MinValue;
        }

        internal Door(Point point, ushort mapId, bool closed)
        {
            MSourceX = point.X;
            MSourceY = point.Y;
            MSourceMapId = mapId;
            Closed = closed;
            LastClick = DateTime.MinValue;
        }

        internal Door(short x, short y, ushort mapId, bool closed)
        {
            MSourceX = x;
            MSourceY = y;
            MSourceMapId = mapId;
            Closed = closed;
            LastClick = DateTime.MinValue;

        }
    }
}
