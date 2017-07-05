using System;

namespace Chaos.Objects
{
    internal abstract class VisibleObject : WorldObject
    {
        internal ushort Sprite { get; }
        internal Point Point { get; set; }
        internal Map Map { get; set; }

        internal VisibleObject(uint id, string name, ushort sprite, Point point, Map map)
          : base(id, name)
        {
            Sprite = sprite;
            Point = point;
            Map = map;
        }

        internal bool WithinRange(Point p) => Point.Distance(p) < 12;
        internal bool WithinRange(VisibleObject v) => Point.Distance(v.Point) < 12;
    }
}
