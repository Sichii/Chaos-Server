using System.Collections.Generic;

namespace Chaos
{
    internal struct MetafileNode
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
