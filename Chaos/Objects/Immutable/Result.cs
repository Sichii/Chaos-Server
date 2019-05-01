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

namespace Chaos
{
    /// <summary>
    /// Object representing whether or not something succeeded, and the error or success text associated with that action.
    /// </summary>
    internal sealed class Result
    {
        internal bool Success;
        internal string Message;

        public static implicit operator Result(ValueTuple<bool, string> pair) => new Result(pair.Item1, pair.Item2);

        internal Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
