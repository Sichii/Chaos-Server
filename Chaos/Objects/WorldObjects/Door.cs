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
        internal new static readonly Type TypeRef = typeof(Door);

        internal bool RecentlyClicked => DateTime.UtcNow.Subtract(LastClick).TotalSeconds < 1.5;
        internal bool Closed { get; set; }
        public bool OpenRight { get; }
        internal DateTime LastClick { get; set; }

        /// <summary>
        /// Master constructor for an object representing an in-game door.
        /// </summary>
        public Door(Location location, bool closed, bool openRight)
            :base(string.Empty, location)
        {
            Closed = closed;
            OpenRight = openRight;
            LastClick = DateTime.MinValue;
        }

        /// <summary>
        /// Toggles whether the door is opened or not.
        /// </summary>
        internal bool Toggle()
        {
            if (!RecentlyClicked)
            {
                Closed = !Closed;
                return true;
            }

            return false;
        }
    }
}
