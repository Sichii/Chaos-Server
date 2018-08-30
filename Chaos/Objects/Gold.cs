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
    /// Represents a gold pile in-game. Gold is handled slightly differently than other ground items.
    /// </summary>
    internal sealed class Gold : GroundObject
    {
        internal uint Amount { get; set; }

        /// <summary>
        /// Master constructor for an object representing an in-game pile of gold.
        /// </summary>
        internal Gold(ushort sprite, Point point, Map map, uint amount)
          : base((ushort)(sprite + CONSTANTS.ITEM_SPRITE_OFFSET), point, map)
        {
            Amount = amount;
        }
    }
}
