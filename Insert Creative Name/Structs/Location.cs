using System;

namespace Insert_Creative_Name
{
    [Serializable]
    internal struct Location
    {
        internal ushort MapId { get; set; }
        internal short X { get; set; }
        internal short Y { get; set; }

        internal Point Point => new Point(X, Y);

        internal Location(ushort id, short x, short y)
        {
            MapId = id;
            X = x;
            Y = y;
        }

        internal Location(ushort id, Point point)
        {
            MapId = id;
            X = point.X;
            Y = point.Y;
        }

        public static bool operator ==(Location loc1, Location loc2)
        {
            return loc1.Equals(loc2);
        }

        public static bool operator !=(Location loc1, Location loc2)
        {
            return !loc1.Equals(loc2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Location))
                return false;

            Location location = (Location)obj;
            return location.MapId == MapId && location.X == X && location.Y == Y;
        }

        public override int GetHashCode()
        {
            return (MapId << 16) + (X << 8) + Y;
        }

        public override string ToString()
        {
            return $"{MapId} {X},{Y}";
        }
    }
}
