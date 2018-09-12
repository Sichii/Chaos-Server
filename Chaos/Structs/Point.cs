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

using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct Point
    {
        [JsonProperty]
        public ushort X;
        [JsonProperty]
        public ushort Y;

        /// <summary>
        /// Json & Master constructor for a structure representing a point within a map.
        /// </summary>
        [JsonConstructor]
        private Point(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Point(ValueTuple<int, int> vTuple) => new Point((ushort)vTuple.Item1, (ushort)vTuple.Item2);

        /// <summary>
        /// Returns the equivalent of no point.
        /// </summary>
        public static Point None => (ushort.MaxValue, ushort.MaxValue);

        public static bool operator ==(Point pt1, Point pt2) => pt1.Equals(pt2);
        public static bool operator !=(Point pt1, Point pt2) => !pt1.Equals(pt2);

        /// <summary>
        /// Gets this point's distance from another point.
        /// </summary>
        internal int Distance(Point pt) => Distance(pt.X, pt.Y);

        /// <summary>
        /// Gets this point's distance from another point.
        /// </summary>
        internal int Distance(ushort x, ushort y) => Math.Abs(x - X) + Math.Abs(y - Y);

        public override int GetHashCode() => (X << 8) + Y;
        public override string ToString() => $@"({X}, {Y})";

        /// <summary>
        /// Moves this point in a given direction, offsetting the x or y co-ordinate accordingly.
        /// </summary>
        internal void Offset(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    Y--;
                    break;
                case Direction.East:
                    X++;
                    break;
                case Direction.South:
                    Y++;
                    break;
                case Direction.West:
                    X--;
                    break;
            }
        }

        /// <summary>
        /// Returns a new point that has been offset in a given direction.
        /// </summary>
        internal Point NewOffset(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return (X, Y - 1);
                case Direction.East:
                    return (X + 1, Y);
                case Direction.South:
                    return (X, Y + 1);
                case Direction.West:
                    return (X - 1, Y);
                default:
                    return None;
            }
        }

        /// <summary>
        /// Returns the directional relation between this point and another point.
        /// The direction this point is from the given point.
        /// </summary>
        internal Direction Relation(Point point)
        {
            Direction direction = Direction.Invalid;
            int degree = 0;
            if (Y < point.Y && point.Y - Y > degree)
            {
                degree = point.Y - Y;
                direction = Direction.North;
            }
            if (X > point.X && X - point.X > degree)
            {
                degree = point.X - X;
                direction = Direction.East;
            }
            if (Y > point.Y && Y - point.Y > degree)
            {
                degree = Y - point.Y;
                direction = Direction.South;
            }
            if (X < point.X && point.X - X > degree)
            {
                degree = point.X - X;
                direction = Direction.West;
            }

            return direction;
        }

        public override bool Equals(object obj)
        {
            try
            {
                var point = (Point)obj;
                return GetHashCode() == point.GetHashCode();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse a point from a given string.
        /// </summary>
        public static bool TryParse(string str, out Point point)
        {
            point = None;
            Match m = Regex.Match(str, @"\((\d+), (\d+)\)");

            return m.Success && ushort.TryParse(m.Groups[1].Value, out point.X) && ushort.TryParse(m.Groups[2].Value, out point.Y);
        }
    }
}
