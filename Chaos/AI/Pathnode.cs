using System;

namespace Chaos
{
    internal sealed class Pathnode : IEquatable<Pathnode>
    {
        internal Point Point { get; }
        internal Pathnode[] Neighbors { get; }

        internal Pathnode Parent { get; set; }
        internal bool Open { get; set; }
        internal bool Closed { get; set; }
        internal bool IsWall { get; set; }
        internal bool HasCreature { get; set; }
        internal int Aggregate { get; set; }

        internal Pathnode(Point point)
        {
            Point = point;
            Neighbors = new Pathnode[4];
        }

        public bool Equals(Pathnode other) => !(other is null) && Point.GetHashCode() == other.Point.GetHashCode();
    }
}
