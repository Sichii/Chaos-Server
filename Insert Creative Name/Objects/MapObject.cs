using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal abstract class MapObject
    {
        protected Point MSourcePoint => new Point(MSourceX, MSourceY);
        protected Location MSourceLocation => new Location(MSourceMapId, MSourcePoint);
        protected short MSourceX;
        protected short MSourceY;
        protected ushort MSourceMapId;
    }
}
