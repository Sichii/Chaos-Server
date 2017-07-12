using System;
using System.Collections.Generic;

namespace Chaos
{
    internal class GameTime
    {
        private const long TICKS_YEAR = 13140000000000;
        private const long TICKS_MONTH = 1080000000000;
        private const long TICKS_DAY = 36000000000;
        private const long TICKS_HOUR = 1500000000;
        private const long TICKS_MINUTE = 25000000;
        private List<string> Months = new List<string>() { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private long Remainder { get; }
        internal ushort Year { get; }
        internal byte Month { get; }
        internal byte Day { get; }
        internal byte Hour { get; }
        internal byte Minute { get; }
        internal long Ticks { get; }

        /// <summary>
        /// Object representing the serverside construct of GameTime, mimicing DateTime
        /// </summary>
        /// <param name="ticks">Base measure of time.</param>
        private GameTime(long ticks)
        {
            Ticks = ticks;
            Year = (ushort)(ticks / TICKS_YEAR);
            ticks -= Year * TICKS_YEAR;

            Month = (byte)(ticks / TICKS_MONTH);
            ticks -= Month * TICKS_MONTH;

            Day = (byte)(ticks / TICKS_DAY);
            ticks -= Day * TICKS_DAY;

            Hour = (byte)(ticks / TICKS_HOUR);
            ticks -= Hour * TICKS_HOUR;

            Minute = (byte)(ticks / TICKS_MINUTE);
            ticks -= Minute * TICKS_MINUTE;

            Remainder = ticks;
        }

        /// <summary>
        /// Starting date of the server.
        /// </summary>
        private static DateTime Origin => new DateTime(2017, 6, 20);
        /// <summary>
        /// Gets the current ingame time.
        /// </summary>
        internal static GameTime Now => FromDateTime(DateTime.UtcNow);
        /// <summary>
        /// Converts a DateTime object to GameTime.
        /// </summary>
        /// <param name="dTime">DateTimeobject to be converted.</param>
        internal static GameTime FromDateTime(DateTime dTime) => new GameTime(dTime.Subtract(Origin).Ticks);
        /// <summary>
        /// Converts a GameTime object to DateTime.
        /// </summary>
        internal DateTime ToDateTime() => new DateTime(Ticks + Origin.Ticks);
        /// <summary>
        /// Custom <see cref="ToString()"/> method, that will print like DateTime does.
        /// </summary>
        /// <param name="format">Optional string format guide.</param>
        internal string ToString(string format = "") => !string.IsNullOrEmpty(format) ? new DateTime(Ticks*24).ToString(format) : new DateTime(Ticks*24).ToString("d MMM y");
    }
}
