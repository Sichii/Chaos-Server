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
    internal sealed class Menu : IEnumerable<Pursuit>
    {
        public int Count => Pursuits.Count;
        public IEnumerator<Pursuit> GetEnumerator() => Pursuits.Select(p => p.Value).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal Pursuit this[PursuitIds pid] => Pursuits.FirstOrDefault(p => p.Key == pid).Value;
        internal string Text { get; }
        internal MenuType Type { get; }
        internal List<KeyValuePair<PursuitIds, Pursuit>> Pursuits { get; }

        internal Menu(IEnumerable<Pursuit> pursuits, MenuType type, string text)
        {
            Pursuits = new List<KeyValuePair<PursuitIds, Pursuit>>(pursuits.ToDictionary(p => p.PursuitId, p => p));
            Type = type;
            Text = text;
        }
    }
}
