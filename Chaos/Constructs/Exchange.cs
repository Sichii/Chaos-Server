using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chaos
{
    internal class Exchange
    {
        internal int ExchangeId { get; }

        internal Exchange()
        {
            ExchangeId = Interlocked.Increment(ref Server.NextId);
        }
    }
}
