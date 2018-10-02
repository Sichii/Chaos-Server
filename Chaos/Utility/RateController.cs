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
using System.Diagnostics;
using System.Threading.Tasks;

namespace Chaos
{
    /// <summary>
    /// An object used to control the rate of execution.
    /// </summary>
    internal sealed class RateController
    {
        private static readonly Stopwatch Timer = Stopwatch.StartNew();
        private static readonly long TicksPerMS = Stopwatch.Frequency / 1000;
        private readonly int DesiredRateMS;
        private int RolloverMS;
        private long LastRequestTicks;
        
        /// <summary>
        /// Creates a new RateLimiter and sets the initial time.
        /// </summary>
        /// <param name="executionsPerSecond">The desired number of executions per second.</param>
        internal RateController(int executionsPerSecond)
        {
            DesiredRateMS = 1000/executionsPerSecond;
            LastRequestTicks = Timer.ElapsedTicks;
        }

        /// <summary>
        /// Gets the time elapsed since the last call of GetTimeout()
        /// </summary>
        private int ElapsedMS => (int)((Timer.ElapsedTicks - LastRequestTicks) / TicksPerMS);

        /// <summary>
        /// Returns a calculated timout based on the time between the set and this call.
        /// </summary>
        private int CalculateTimeout()
        {
            int executionTimeMS = ElapsedMS;

            if (executionTimeMS > DesiredRateMS)
                RolloverMS += executionTimeMS - DesiredRateMS;
            else
                RolloverMS -= Math.Min(RolloverMS, DesiredRateMS - executionTimeMS);

            LastRequestTicks = Timer.ElapsedTicks;
            return Math.Max(0, DesiredRateMS - executionTimeMS - RolloverMS);
        }

        internal async Task ThrottleAsync()
        {
            await Task.Delay(CalculateTimeout());

            LastRequestTicks += ElapsedMS * TicksPerMS;
        }
    }
}
