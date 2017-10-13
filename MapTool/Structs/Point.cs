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

using System;
using System.Text.RegularExpressions;

namespace MapTool
{
    internal struct Point
    {
        internal ushort X { get; set; }
        internal ushort Y { get; set; }

        internal Point(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        internal Point(short x, short y)
        {
            X = (ushort)x;
            Y = (ushort)y;
        }

        public static bool operator ==(Point pt1, Point pt2) => pt1.Equals(pt2);
        public static bool operator !=(Point pt1, Point pt2) => !pt1.Equals(pt2);
        internal int Distance(Point pt) => Distance(pt.X, pt.Y);
        internal int Distance(ushort x, ushort y) => Math.Abs(x - X) + Math.Abs(y - Y);
        public override int GetHashCode() => (X << 16) + Y;
        public override string ToString() => $@"{X},{Y}";


        public override bool Equals(object obj)
        {
            if (!(obj is Point))
                return false;

            Point point = (Point)obj;
            return point.X == X && point.Y == Y;
        }

        public static Point Parse(string str)
        {
            Match m = Regex.Match(str, @"\((\d+),(\d+)\)");
            return new Point(ushort.Parse(m.Groups[1].Value), ushort.Parse(m.Groups[2].Value));
        }
    }
}
