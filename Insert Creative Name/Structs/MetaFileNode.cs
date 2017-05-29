using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal class MetafileNode
    {
        internal string Name { get; private set; }
        internal List<string> Properties { get; private set; }

        internal MetafileNode(string name)
        {
            Name = name;
            Properties = new List<string>();
        }
    }
}
