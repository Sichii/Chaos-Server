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

using Newtonsoft.Json;

namespace Chaos
{
    internal sealed class Personal
    {
        [JsonProperty]
        internal byte[] Portrait { get; set; }
        [JsonProperty]
        internal string Message { get; set; }

        /// <summary>
        /// Object representing the Portrait and Profile Message.
        /// </summary>
        [JsonConstructor]
        internal Personal(byte[] portrait, string message)
        {
            Portrait = portrait;
            Message = message;
        }

    }
}
