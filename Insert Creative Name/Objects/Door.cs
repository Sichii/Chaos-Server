using System;

namespace Insert_Creative_Name.Objects
{
    internal sealed class Door : MapObject
    {
        internal bool RecentlyClicked => DateTime.UtcNow.Subtract(LastClick).TotalSeconds < 1.5;
        internal bool Closed { get; set; }
        internal DateTime LastClick { get; set; }

        internal Door(Location location, bool closed)
            :base(location)
        {
            Closed = closed;
            LastClick = DateTime.MinValue;
        }

        internal Door(ushort mapId, Point point, bool closed)
            :base(mapId, point)
        {
            Closed = closed;
            LastClick = DateTime.MinValue;
        }

        internal Door(ushort mapId, short x, short y, bool closed)
            :base(mapId, x, y)
        {
            Closed = closed;
            LastClick = DateTime.MinValue;

        }
    }
}
