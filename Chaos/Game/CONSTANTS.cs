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
        internal static Location STARTING_LOCATION = new Location(5031, 20, 20);
    }
}
