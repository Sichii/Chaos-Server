using System.Collections.Generic;

namespace Chaos
{
    internal struct MetafileNode
    {
        internal string Name { get; }
        internal List<string> Properties { get; set; }

        internal MetafileNode(string name)
        {
            Name = name;
            Properties = new List<string>();
        }
    }
}
