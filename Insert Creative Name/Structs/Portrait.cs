using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal struct Portrait
    {
        internal byte[] Data { get; }

        internal Portrait(byte[] data)
        {
            Data = data;
        }
    }
}
