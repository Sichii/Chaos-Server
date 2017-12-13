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
using Newtonsoft.Json;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class GameTime
    {
        private long Remainder { get; }
        internal ushort Year { get; }
        internal byte Month { get; }
        internal byte Day { get; }
        internal byte Hour { get; }
        internal byte Minute { get; }
        [JsonProperty]
        internal long Ticks { get; }

        /// <summary>
        /// Object representing the serverside construct of GameTime, mimicing DateTime
        /// </summary>
        /// <param name="ticks">Base measure of time.</param>
        [JsonConstructor]
        private GameTime(long ticks)
        {
            Ticks = ticks;
            Year = (ushort)(ticks / CONSTANTS.YEAR_TICKS);
            ticks -= Year * CONSTANTS.YEAR_TICKS;

            Month = (byte)(ticks / CONSTANTS.MONTH_TICKS);
            ticks -= Month * CONSTANTS.MONTH_TICKS;

            Day = (byte)(ticks / CONSTANTS.DAY_TICKS);
            ticks -= Day * CONSTANTS.DAY_TICKS;

            Hour = (byte)(ticks / CONSTANTS.HOUR_TICKS);
            ticks -= Hour * CONSTANTS.HOUR_TICKS;

            Minute = (byte)(ticks / CONSTANTS.MINUTE_TICKS);
            ticks -= Minute * CONSTANTS.MINUTE_TICKS;

            Remainder = ticks;
        }

        private string GetDaySuffix =>
            Day % 10 == 1 && Day != 11 ? "st" :
            Day % 10 == 2 && Day != 12 ? "nd" :
            Day % 10 == 3 && Day != 13 ? "rd" : "th";

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
        internal string ToString(string format = "") => $@"Year {(!string.IsNullOrEmpty(format) ? new DateTime(Ticks*24).ToString(format) : new DateTime(Ticks*24).ToString($@"y, MMM d"))}{GetDaySuffix}";
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
