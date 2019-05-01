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

using System;

namespace Chaos
{
    /// <summary>
    /// Represents an object or tile on a map that moves a user.
    /// </summary>
    public sealed class Warp : MapObject
    {
        internal new static readonly Type TypeRef = typeof(Warp);

        public Location TargetLocation { get; }
        public Point TargetPoint => TargetLocation.Point;

        /// <summary>
        /// Optional constructor for an unsourced warp.
        /// </summary>
        internal static Warp Unsourced(Location targetLocation) => new Warp(Location.None, targetLocation);

        /// <summary>
        /// Creates a warp that will send a user home, and returns it.
        /// </summary>
        internal static Warp Home(User user)
        {
            Location home = Location.None;
            switch(user.Nation)
            {
                case Nation.None:
                    home = CONSTANTS.NO_NATION_LOCATION;
                    break;
                case Nation.Suomi:
                    home = CONSTANTS.SUOMI_LOCATION;
                    break;
                case Nation.Loures:
                    home = CONSTANTS.LOURES_LOCATION;
                    break;
                case Nation.Mileth:
                    home = CONSTANTS.MILETH_LOCATION;
                    break;
                case Nation.Tagor:
                    home = CONSTANTS.TAGOR_LOCATION;
                    break;
                case Nation.Rucesion:
                    home = CONSTANTS.RUCESION_LOCATION;
                    break;
                case Nation.Noes:
                    home = CONSTANTS.NOES_LOCATION;
                    break;
            }

            return new Warp(user.Location, home);
        }

        /// <summary>
        /// Creates a warp that will send a user to purgatory, and returns it.
        /// </summary>
        internal static Warp Death(User user) => new Warp(user.Location, CONSTANTS.DEATH_LOCATION);

        /// <summary>
        /// Master constructor for an object or tile on a map that moves a user.
        /// </summary>
        internal Warp(Location sourceLocation, Location targetLocation)
            :base(string.Empty, sourceLocation)
        {
            TargetLocation = targetLocation;
        }

        /// <summary>
        /// Optional constructor for a warp taking only primitives.
        /// </summary>
        public Warp(ushort sourceMapId, ushort sourceX, ushort sourceY, ushort targetMapId, ushort targetX, ushort targetY)
            :this((sourceMapId, sourceX, sourceY), (targetMapId, targetX, targetY))
        {
        }

        public override string ToString() => $@"{Location.ToString()} => {TargetLocation.ToString()}";
    }
}
