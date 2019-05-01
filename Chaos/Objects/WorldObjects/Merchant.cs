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
    /// Represents an in-game merchant.
    /// </summary>
    internal sealed class Merchant : Creature
    {
        internal ushort NextDialogId { get; }
        internal MerchantMenu Menu { get; }
        internal override uint CurrentHP { get => int.MaxValue; set { } }

        internal override uint MaximumHP => int.MaxValue;
        internal override byte HealthPercent => 100;

        /// <summary>
        /// Master constructor for an object representing an in-game merchant.
        /// </summary>
        internal Merchant(string name, Location location, ushort sprite, Direction direction, ushort nextDialogId = 0, MerchantMenu menu = null)
            : base(name, location, (ushort)(sprite + CONSTANTS.CREATURE_SPRITE_OFFSET), CreatureType.Merchant, direction)
        {
            NextDialogId = nextDialogId;
            Menu = menu;
        }
    }
}
