using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace Chaos
{
    internal struct Point
    {
        [JsonProperty]
        internal ushort X { get; set; }
        [JsonProperty]
        internal ushort Y { get; set; }

        [JsonConstructor]
        internal Point(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Point pt1, Point pt2) => pt1.Equals(pt2);
        public static bool operator !=(Point pt1, Point pt2) => !pt1.Equals(pt2);
        internal int Distance(Point pt) => Distance(pt.X, pt.Y);
        internal int Distance(ushort x, ushort y) => Math.Abs(x - X) + Math.Abs(y - Y);
        public override int GetHashCode() => (X << 16) + Y;
        public override string ToString() => $@"{X},{Y}";


        internal void Offset(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    --Y;
                    break;
                case Direction.East:
                    ++X;
                    break;
                case Direction.South:
                    ++Y;
                    break;
                case Direction.West:
                    --X;
                    break;
            }
        }

        internal Point Offsetter(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(X, (ushort)(Y - 1));
                case Direction.East:
                    return new Point((ushort)(X + 1), Y);
                case Direction.South:
                    return new Point((X), (ushort)(Y + 1));
                case Direction.West:
                    return new Point((ushort)(X - 1), Y);
                default:
                    return new Point(0, 0);
            }
        }

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
