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
