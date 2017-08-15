namespace MapTool
{
    internal struct Location
    {
        internal ushort MapId { get; set; }
        internal ushort X { get; set; }
        internal ushort Y { get; set; }
        internal Point Point => new Point(X, Y);

        internal Location(ushort id, ushort x, ushort y)
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

        public static bool operator ==(Location loc1, Location loc2) => loc1.Equals(loc2);
        public static bool operator !=(Location loc1, Location loc2) => !loc1.Equals(loc2);
        public override int GetHashCode() => (MapId << 16) + (X << 8) + Y;
        public override string ToString() => $"{MapId} {X},{Y}";

        public override bool Equals(object obj)
        {
            if (!(obj is Location))
                return false;

            Location location = (Location)obj;
            return location.MapId == MapId && location.X == X && location.Y == Y;
        }
    }
}
