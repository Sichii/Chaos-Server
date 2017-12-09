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
using System.Collections.Generic;

namespace Chaos
{
    internal sealed class GameTime
    {
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
            Year = (ushort)(ticks / CONSTANTS.TICKS_YEAR);
            ticks -= Year * CONSTANTS.TICKS_YEAR;

            Month = (byte)(ticks / CONSTANTS.TICKS_MONTH);
            ticks -= Month * CONSTANTS.TICKS_MONTH;

            Day = (byte)(ticks / CONSTANTS.TICKS_DAY);
            ticks -= Day * CONSTANTS.TICKS_DAY;

            Hour = (byte)(ticks / CONSTANTS.TICKS_HOUR);
            ticks -= Hour * CONSTANTS.TICKS_HOUR;

            Minute = (byte)(ticks / CONSTANTS.TICKS_MINUTE);
            ticks -= Minute * CONSTANTS.TICKS_MINUTE;

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
        internal LightLevel TimeOfDay
        {
            get
            {
                if (Hour >= 10 && Hour <= 14)
                    return LightLevel.Lightest;
                else if (Hour >= 8 && Hour <= 16)
                    return LightLevel.Lighter;
                else if (Hour >= 6 && Hour <= 18)
                    return LightLevel.Light;
                else if (Hour >= 4 && Hour <= 20)
                    return LightLevel.Dark;
                else if (Hour >= 2 && Hour <= 22)
                    return LightLevel.Darker;
                else
                    return LightLevel.Darkest;
            }
        }
    }
}
