using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapTool
{
    internal class MapTreeNode : TreeNode
    {
        internal Map Map { get; }

        internal MapTreeNode(Map map, string name)
            : base(name)
        {
            Map = map;
            Name = map.Name;
        }
    }
}
