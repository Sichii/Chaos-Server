﻿// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

namespace Chaos
{
    internal sealed class PursuitMenuItem
    {
        internal PursuitIds PursuitId { get; }
        internal string Text { get; }

        /// <summary>
        /// Master constructor for a structure representing an item for a pursuit menu.
        /// </summary>
        internal PursuitMenuItem(PursuitIds pursuitId, string text)
        {
            PursuitId = pursuitId;
            Text = text;
        }
    }
}
