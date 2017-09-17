using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal sealed class Menu : IEnumerable<Pursuit>
    {
        public int Count => Pursuits.Values.Count;
        public IEnumerator<Pursuit> GetEnumerator() => Pursuits.Values.ToList().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal Pursuit this[PursuitIds pid] => Pursuits[pid];
        internal string Text { get; }
        internal MenuType Type { get; }
        internal SortedDictionary<PursuitIds, Pursuit> Pursuits { get; }

        internal Menu(List<Pursuit> pursuits, MenuType type, string text)
        {
            Pursuits = new SortedDictionary<PursuitIds, Pursuit>(pursuits.ToDictionary(p => p.PursuitId, p => p));
            Type = type;
            Text = text;
        }
    }
}
