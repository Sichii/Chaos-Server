using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal sealed class Warp : MapObject
    {
        internal short SourceX => MSourceX;
        internal short SourceY => MSourceY;
        internal short TargetX { get; }
        internal short TargetY { get; }
        internal ushort TargetMapId { get; }
        internal Point SourcePoint => MSourcePoint;
        internal Location SourceLocation => MSourceLocation;
        internal Point TargetPoint => new Point(TargetX, TargetY);
        internal Location TargetLocation => new Location(TargetMapId, TargetX, TargetY);

        internal Warp(short sourceX, short sourceY, short targetX, short targetY, ushort sourceMapId, ushort targetMapId)
        {
            MSourceX = sourceX;
            MSourceY = sourceY;
            TargetX = targetX;
            TargetY = targetY;
            MSourceMapId = sourceMapId;
            TargetMapId = targetMapId;
        }
    }
}
