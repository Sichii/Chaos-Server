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
        internal byte[] Portrait { get; set; }
        internal string Message { get; set; }

        /// <summary>
        /// Master constructor for the object representing the Portrait and Profile Message.
        /// </summary>
        internal Personal(byte[] portrait, string message)
        {
            Portrait = portrait;
            Message = message;
        }

    }
}
