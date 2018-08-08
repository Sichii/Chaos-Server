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
using System.Linq;

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
        internal static Location STARTING_LOCATION = new Location(18000, 5, 6);
        internal static Location DEATH_LOCATION = new Location(5031, 15, 15);
        //nation locations
        internal static Location NO_NATION_LOCATION = new Location(8984, 10, 10);
        internal static Location SUOMI_LOCATION = new Location(8984, 10, 10);
        internal static Location LOURES_LOCATION = new Location(8984, 10, 10);
        internal static Location MILETH_LOCATION = new Location(8984, 10, 10);
        internal static Location TAGOR_LOCATION = new Location(8984, 10, 10);
        internal static Location RUCESION_LOCATION = new Location(8984, 10, 10);
        internal static Location NOES_LOCATION = new Location(8984, 10, 10);
        //stat to damage / health / etc modifiers will be added here later.
    }
}
