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
    internal class Merchant : Creature
    {
        internal DateTime LastClicked { get; set; }
        internal bool ShouldDisplay => DateTime.UtcNow.Subtract(LastClicked).TotalMilliseconds < 500;
        internal ushort NextDialogId { get; }
        internal MerchantMenu Menu { get; }
        internal override byte HealthPercent => 100;
        internal override uint CurrentHP { get { return int.MaxValue; } set { } }
        internal override uint MaximumHP { get { return int.MaxValue; } }

        internal Merchant(string name, ushort sprite, Point point, Map map, Direction direction, ushort nextDialogId = 0, MerchantMenu menu = null)
            : base(name, (ushort)(sprite + CONSTANTS.MERCHANT_SPRITE_OFFSET), CreatureType.Merchant, point, map, direction)
        {
            NextDialogId = nextDialogId;
            LastClicked = DateTime.MinValue;
            Menu = menu;
        }
    }
}
