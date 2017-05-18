using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal abstract class VisibleObject : WorldObject
    {
        internal ushort Sprite { get; set; }
        internal Point Point { get; set; }
        internal Map Map { get; set; }

        internal VisibleObject(uint id, string name, ushort sprite, Point point, Map map)
          : base(id, name)
        {
            Sprite = sprite;
            Point = point;
            Map = map;
        }

        internal bool WithinRange(VisibleObject vo, int range = 12)
        {
            if (Map == vo.Map)
                return Point.Distance(vo.Point) <= range;
            return false;
        }
    }
}
