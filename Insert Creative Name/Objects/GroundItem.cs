using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal sealed class GroundItem : VisibleObject
    {
        internal bool IsItem { get; }

        internal GroundItem(int id, ushort sprite, Point point, Map map)
          : base(id, string.Empty, sprite, point, map)
        {
            IsItem = true;
        }
    }
}
