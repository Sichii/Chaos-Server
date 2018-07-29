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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal sealed class DialogMenu : IEnumerable<DialogMenuItem>
    {
        public int Count => Options.Count;
        public IEnumerator<DialogMenuItem> GetEnumerator() => Options.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal DialogMenuItem this[int index] => Options[index];
        internal List<DialogMenuItem> Options { get; }

        internal DialogMenu(List<DialogMenuItem> options)
        {
            Options = options;
        }
    }
}
