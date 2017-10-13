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
