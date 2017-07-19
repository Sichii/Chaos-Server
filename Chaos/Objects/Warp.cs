using Newtonsoft.Json;

namespace Chaos.Objects
{
    internal sealed class Warp : MapObject
    {
        internal ushort SourceX => X;
        internal ushort SourceY => Y;
        internal ushort TargetX { get; }
        internal ushort TargetY { get; }
        internal ushort TargetMapId { get; }
        internal Point TargetPoint => new Point(TargetX, TargetY);
        internal Location TargetLocation => new Location(TargetMapId, TargetX, TargetY);

        internal Warp(ushort sourceX, ushort sourceY, ushort targetX, ushort targetY, ushort sourceMapId, ushort targetMapId)
            :base(sourceMapId, sourceX, sourceY)
        {
            TargetMapId = targetMapId;
            TargetX = targetX;
            TargetY = targetY;
        }

        internal Warp(Location sourceLocation, Location targetLocation)
            :base(sourceLocation.MapId, sourceLocation.X, sourceLocation.Y)
        {
            TargetMapId = targetLocation.MapId;
            TargetX = targetLocation.X;
            TargetY = targetLocation.Y;
        }
    }
}
