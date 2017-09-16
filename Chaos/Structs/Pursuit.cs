using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos
{
    internal struct Pursuit
    {
        internal string Text { get; }
        internal ushort PursuitId { get; }
        internal ushort DialogId { get; }

        internal Pursuit(string text, ushort pursuitId, ushort dialogId)
        {
            Text = text;
            PursuitId = pursuitId;
            DialogId = dialogId;
        }
    }
}
