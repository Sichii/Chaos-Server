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

namespace Chaos
{
    internal sealed class GroupBox
    {
        internal User GroupLeader { get; set; }
        internal string Text { get; set; }
        internal byte MaxLevel { get; set; }
        internal byte[] MaxAmounts { get; set; }

        /// <summary>
        /// Base constructor for an object representing an in-game GroupBox.
        /// </summary>
        /// <param name="leader"></param>
        /// <param name="text"></param>
        /// <param name="maxLevel"></param>
        /// <param name="maxAmounts"></param>
        internal GroupBox(User leader, string text, byte maxLevel, byte[] maxAmounts)
        {
            GroupLeader = leader;
            Text = text;
            MaxLevel = maxLevel;
            MaxAmounts = maxAmounts;
        }
    }
}
