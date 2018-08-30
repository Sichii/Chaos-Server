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
    /// Represents a clickable door, which can open or close in game.
    /// </summary>
    public sealed class Door : MapObject
    {
        internal bool RecentlyClicked => DateTime.UtcNow.Subtract(LastClick).TotalSeconds < 1.5;
        internal bool Opened { get; set; }
        public bool OpenRight { get; }
        internal DateTime LastClick { get; set; }

        /// <summary>
        /// Optional constructor that takes a location.
        /// </summary>
        public Door(Location location, bool opened, bool openRight)
            :this(location.MapId, location.Point, opened, openRight)
        {
        }

        /// <summary>
        /// Optional constructor that takes a point.
        /// </summary>
        public Door(ushort mapId, Point point, bool opened, bool openRight)
            :this(mapId, point.X, point.Y, opened, openRight)
        {
        }

        /// <summary>
        /// Master constructor for an object representing an in-game door.
        /// </summary>
        public Door(ushort mapId, ushort x, ushort y, bool opened, bool openRight)
            :base(mapId, x, y)
        {
            Opened = opened;
            OpenRight = OpenRight;
            LastClick = DateTime.MinValue;
        }

        /// <summary>
        /// Toggles whether the door is opened or not.
        /// </summary>
        internal void Toggle() => Opened = !Opened;
    }
}
