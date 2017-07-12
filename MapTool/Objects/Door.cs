using System;

namespace MapTool
{
    internal sealed class Door : MapObject
    {
        internal bool RecentlyClicked => DateTime.UtcNow.Subtract(LastClick).TotalSeconds < 1.5;
        internal bool Opened { get; set; }
        internal bool OpenRight { get; }
        internal DateTime LastClick { get; set; }

        internal Door(Location location, bool opened, bool openRight)
            : base(location)
        {
            Opened = opened;
            OpenRight = OpenRight;
            LastClick = DateTime.MinValue;
        }

        internal Door(ushort mapId, Point point, bool opened, bool openRight)
            : base(mapId, point)
        {
            Opened = opened;
            OpenRight = OpenRight;
            LastClick = DateTime.MinValue;
        }

        internal Door(ushort mapId, ushort x, ushort y, bool opened, bool openRight)
            : base(mapId, x, y)
        {
            Opened = opened;
            OpenRight = OpenRight;
            LastClick = DateTime.MinValue;

        }
    }
}
