using System;

namespace Insert_Creative_Name
{
    public struct Location : IComparable
    {
        public short MapId { get; set; }
        public short X { get; set; }
        public short Y { get; set; }

        public Point Point => new Point(X, Y);

        public Location(short id, short x, short y)
        {
            MapId = id;
            X = x;
            Y = y;
        }

        public Location(short id, Point point)
        {
            MapId = id;
            X = point.X;
            Y = point.Y;
        }

        public static bool operator ==(Location l1, Location l2)
        {
            return l1.Equals(l2);
        }

        public static bool operator !=(Location l1, Location l2)
        {
            return !l1.Equals(l2);
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Location)) return -1;
            Location location = (Location)obj;
            if (MapId == location.MapId)
            {
                if (Y != location.Y) return Y <= location.Y ? -1 : 1;
                if (X == location.X)
                    return 0;
                return X <= location.X ? -1 : 1;
            }
            if (MapId > location.MapId)
                return 1;
            if (MapId < location.MapId)
                return -1;
            return -1;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Location))
                return false;
            Location location = (Location)obj;
            if (location.MapId == MapId && location.X == X)
                return location.Y == Y;
            return false;
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
