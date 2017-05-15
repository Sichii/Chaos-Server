using System;

namespace Insert_Creative_Name
{
    public struct Point
    {
        public short X { get; set; }
        public short Y { get; set; }

        public Point(short x, short y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Point pt1, Point pt2)
        {
            return pt1.Equals(pt2);
        }

        public static bool operator !=(Point pt1, Point pt2)
        {
            return !pt1.Equals(pt2);
        }

        public int Distance(Point pt)
        {
            return Distance(pt.X, pt.Y);
        }

        public int Distance(short x, short y)
        {
            return Math.Abs(x - X) + Math.Abs(y - Y);
        }

        public void Offset(Direction direction)
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

        public Point Offsetter(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(X, (short)(Y - 1));
                case Direction.East:
                    return new Point((short)(X + 1), Y);
                case Direction.South:
                    return new Point((X), (short)(Y + 1));
                case Direction.West:
                    return new Point((short)(X - 1), Y);
                default:
                    return new Point(0, 0);
            }
        }

        public Direction Relation(Point point)
        {
            if (Y < point.Y)
                return Direction.North;
            if (X > point.X)
                return Direction.East;
            if (Y > point.Y)
                return Direction.South;
            return X < point.X ? Direction.West : Direction.Invalid;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
                return false;
            Point point = (Point)obj;
            if (point.X == X)
                return point.Y == Y;
            return false;
        }

        public override int GetHashCode()
        {
            return (X << 16) + Y;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", X, Y);
        }
    }
}
