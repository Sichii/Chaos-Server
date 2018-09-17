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
    /// <summary>
    /// Represents an object or tile on a map that moves a user.
    /// </summary>
    public sealed class Warp : MapObject
    {
        public ushort SourceX => X;
        public ushort SourceY => Y;
        public ushort TargetX { get; }
        public ushort TargetY { get; }
        public ushort TargetMapId { get; }
        public Point TargetPoint => (TargetX, TargetY);
        public Location TargetLocation => (TargetMapId, TargetPoint);

        /// <summary>
        /// Optional constructor for a warp.
        /// </summary>
        internal Warp(Location sourceLocation, Location targetLocation)
            :this(sourceLocation.X, sourceLocation.Y, targetLocation.X, targetLocation.Y, sourceLocation.MapId, targetLocation.MapId)
        {
        }

        /// <summary>
        /// Master constructor for an object or tile on a map that moves a user.
        /// </summary>
        public Warp(ushort sourceX, ushort sourceY, ushort targetX, ushort targetY, ushort sourceMapId, ushort targetMapId)
            :base(sourceMapId, sourceX, sourceY)
        {
            TargetMapId = targetMapId;
            TargetX = targetX;
            TargetY = targetY;
        }
    }
}
