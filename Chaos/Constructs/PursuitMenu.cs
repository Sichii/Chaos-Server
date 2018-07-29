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

namespace Chaos
{
    internal sealed class PursuitMenu : IEnumerable<PursuitMenuItem>
    {
        public int Count => Pursuits.Count;
        public IEnumerator<PursuitMenuItem> GetEnumerator() => Pursuits.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal PursuitMenuItem this[int index] => Pursuits[index];
        internal List<PursuitMenuItem> Pursuits { get; }

        internal PursuitMenu(List<PursuitMenuItem> pursuits)
        {
            Pursuits = pursuits;
        }
    }
}
