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

using System.Collections.Generic;

namespace Chaos
{
    internal static class CONSTANTS
    {
        //gametime
        internal const long YEAR_TICKS = 13140000000000;
        internal const long MONTH_TICKS = 1080000000000;
        internal const long DAY_TICKS = 36000000000;
        internal const long HOUR_TICKS = 1500000000;
        internal const long MINUTE_TICKS = 25000000;

        //game
        internal const int ITEM_SPRITE_OFFSET = 32768;
        internal const int MERCHANT_SPRITE_OFFSET = 16384;
        internal const int PICKUP_RANGE = 4;
        internal const int DROP_RANGE = 4;
        internal const int GLOBAL_SKILL_COOLDOWN_MS = 750;
        internal const int GLOBAL_ITEM_COOLDOWN_MS = 500;
        internal const int GLOBAL_SPELL_COOLDOWN_MS = 250;
        internal const int REFRESH_DELAY_MS = 1000;
        internal static Location STARTING_LOCATION = new Location(8984, 10, 10);

        /*
        internal static Dictionary<EquipmentSlot, Dictionary<ushort, ushort>> ITEM_SPRITE_TO_EQUIPMENT_INDEX = new Dictionary<EquipmentSlot, Dictionary<ushort, ushort>>()
        {
            {
                EquipmentSlot.Weapon, new Dictionary<ushort, ushort>()
                {
                    { 86, 1 }, { 87, 2 }, { 88, 3 }, { 89, 4 }, { 90, 5 }, { 91, 6 }, { 132, 7 }, { 133, 8 }, { 134, 9 }, { 169, 10 }, { 170, 11 }, { 171, 12 }, { 172, 13 },
                    { 173, 14 }, { 174, 15 }, { 175, 16 }, { 176, 17 }, { 177, 18 }, { 178, 19 }, { 179, 20 }, { 180, 21 }, { 181, 22 }, { 182, 23 }, { 183, 24 }, { 184, 25 },
                    { 185, 26 }, { 186, 27 }, { 221, 28 }, { 00000, 29 }, { 254, 30 }, { 255, 31 }, { 256, 32 }, { 257, 33 }, { 00000, 34 }, { 00000, 35 }, { 00000, 36 }
                }
            }
        };
        */
    }
}
