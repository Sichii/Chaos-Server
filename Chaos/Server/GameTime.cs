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
using Newtonsoft.Json;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class GameTime
    {
        private readonly DateTime DateTime;
        [JsonProperty]
        private readonly long Ticks;

        internal int Year => DateTime.Year;
        internal int Month => DateTime.Month;
        internal int Day => DateTime.Day;
        internal int Hour => DateTime.Hour;
        internal int Minute => DateTime.Minute;

        /// <summary>
        /// Json & Master constructor for an object representing the serverside construct of time. Mimics DateTime, except at 24x speed, and starting from an origin(server launch date).
        /// </summary>
        /// <param name="ticks">Base measure of time.</param>
        [JsonConstructor]
        private GameTime(long ticks)
        {
            DateTime = new DateTime(ticks);
            Ticks = ticks;
        }

        /// <summary>
        /// Gets the proper suffix for a day, based on the number.
        /// </summary>
        private string GetDaySuffix => (Day % 10 == 1 && Day != 11) ? "st" 
            : (Day % 10 == 2 && Day != 12) ? "nd"
            : (Day % 10 == 3 && Day != 13) ? "rd" 
            : "th";

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
        internal static GameTime FromDateTime(DateTime dTime) => new GameTime(dTime.Subtract(Origin).Ticks*24);

        /// <summary>
        /// Converts a GameTime object to DateTime.
        /// </summary>
        internal DateTime ToDateTime() => new DateTime((Ticks / 24) + Origin.Ticks);

        /// <summary>
        /// Custom method that will print the current time like DateTime does.
        /// </summary>
        /// <param name="format">Optional string format guide.</param>
        internal string ToString(string format = "") => $@"Year {(!string.IsNullOrEmpty(format) ? DateTime.ToString(format) : DateTime.ToString($@"y, MMM d"))}{GetDaySuffix}";

        /// <summary>
        /// Gets the appropriate level of light for the time of day.
        /// </summary>
        internal LightLevel TimeOfDay => (Hour >= 10 && Hour <= 14) ? LightLevel.Lightest
            : (Hour >= 8 && Hour <= 16) ? LightLevel.Lighter
            : (Hour >= 6 && Hour <= 18) ? LightLevel.Light
            : (Hour >= 4 && Hour <= 20) ? LightLevel.Dark 
            : (Hour >= 2 && Hour <= 22) ? LightLevel.Darker 
            : LightLevel.Darkest;
    }
}
