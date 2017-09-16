using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos
{
    internal sealed class Menu
    {
        internal MenuType Type { get; }
        internal Dialog this[ushort pursuitID] => DialogList.FirstOrDefault(kvp => kvp.Key.PursuitId == pursuitID).Value;
        private Dictionary<Pursuit, Dialog> DialogList = new Dictionary<Pursuit, Dialog>();

        internal Menu(List<Pursuit> pursuits, List<Dialog> dialogs, MenuType type)
        {
            DialogList = pursuits.Zip(dialogs, (k, v) => new { k, v }).ToDictionary(kvp => kvp.k, kvp => kvp.v);
            Type = type;
        }
    }
}
