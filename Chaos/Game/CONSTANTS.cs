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
    internal static class CONSTANTS
    {
        //gametime
        internal const long TICKS_YEAR = 13140000000000;
        internal const long TICKS_MONTH = 1080000000000;
        internal const long TICKS_DAY = 36000000000;
        internal const long TICKS_HOUR = 1500000000;
        internal const long TICKS_MINUTE = 25000000;

        //game
        internal const int ITEM_SPRITE_OFFSET = 32768;
        internal const int MERCHANT_SPRITE_OFFSET = 16384;
        internal const int PICKUP_RANGE = 4;
        internal const int DROP_RANGE = 4;
        internal const int GLOBAL_SKILL_COOLDOWN = 750;
        internal const int GLOBAL_ITEM_COOLDOWN = 500;
        internal const int GLOBAL_SPELL_COOLDOWN = 250;
        internal static Location STARTING_LOCATION = new Location(8984, 10, 10);
    }
}
