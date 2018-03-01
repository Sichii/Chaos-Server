// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

namespace Chaos
{
    public sealed class Warp : MapObject
    {
        public ushort SourceX => X;
        public ushort SourceY => Y;
        public ushort TargetX { get; }
        public ushort TargetY { get; }
        public ushort TargetMapId { get; }
        public Point TargetPoint => new Point(TargetX, TargetY);
        public Location TargetLocation => new Location(TargetMapId, TargetX, TargetY);

        public Warp(ushort sourceX, ushort sourceY, ushort targetX, ushort targetY, ushort sourceMapId, ushort targetMapId)
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
