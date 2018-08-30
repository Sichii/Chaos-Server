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
    internal struct Tile
    {
        internal byte[] sotp => Properties.Resources.sotp;
        internal short Background { get; }
        internal short LeftForeground { get; }
        internal short RightForeground { get; }
        public bool IsWall => (LeftForeground > 0 && (sotp[LeftForeground - 1] & 15) == 15) || (RightForeground > 0 && (sotp[RightForeground - 1] & 15) == 15);

        /// <summary>
        /// Master constructor for a structure representing a single tile on a map, containing it's visual data.
        /// </summary>
        internal Tile(short background, short leftForeground, short rightForeground)
        {
            Background = background;
            LeftForeground = leftForeground;
            RightForeground = rightForeground;
        }
    }
}
