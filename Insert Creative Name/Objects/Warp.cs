namespace Chaos.Objects
{
    internal sealed class Warp : MapObject
    {
        internal short SourceX => X;
        internal short SourceY => Y;
        internal short TargetX { get; }
        internal short TargetY { get; }
        internal ushort TargetMapId { get; }
        internal Point TargetPoint => new Point(TargetX, TargetY);
        internal Location TargetLocation => new Location(TargetMapId, TargetX, TargetY);

        internal Warp(short sourceX, short sourceY, short targetX, short targetY, ushort sourceMapId, ushort targetMapId)
            :base(sourceMapId, sourceX, sourceY)
        {
            TargetMapId = targetMapId;
            TargetX = targetX;
            TargetY = targetY;
        }
    }
}
