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

using System.Linq;

namespace Chaos
{
    internal sealed class MerchantMenu
    {
        public int Count => Pursuits.Count;
        internal string this[PursuitIds pid] => Pursuits.FirstOrDefault(p => p.PursuitId == pid).Text;
        internal string Text { get; }
        internal MenuType Type { get; }
        internal PursuitMenu Pursuits { get; }
        internal DialogMenu Dialogs { get; }

        /// <summary>
        /// Object representing a merchant's menu.
        /// </summary>
        /// <param name="text">The message in the menu window.</param>
        /// <param name="type">The type of menu.</param>
        /// <param name="pursuits">A menu of pursuits, which have instant effects, or other merchant menus.</param>
        /// <param name="dialogs">A menu of dialogs, that can have instant effects or sequential dialogs.</param>
        internal MerchantMenu(string text, MenuType type, PursuitMenu pursuits, DialogMenu dialogs = null)
        {
            Pursuits = pursuits;
            Dialogs = dialogs;
            Type = type;
            Text = text;
        }

        /// <summary>
        /// Object representing a merchant's menu.
        /// </summary>
        /// <param name="text">The message in the menu window.</param>
        /// <param name="type">The type of menu.</param>
        /// <param name="dialogs">A menu of dialogs, that can have instant effects or sequential dialogs.</param>
        /// <param name="pursuits">A menu of pursuits, which have instant effects, or other merchant menus.</param>
        internal MerchantMenu(string text, MenuType type, DialogMenu dialogs, PursuitMenu pursuits = null)
        {
            Pursuits = pursuits;
            Dialogs = dialogs;
            Type = type;
            Text = text;
        }
    }
}
