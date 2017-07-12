using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapTool
{
    internal class DoorTreeNode : TreeNode
    {
        internal Door Door { get; }

        internal DoorTreeNode(Door door, string name)
            : base(name)
        {
            Door = door;
            Name = name;
        }
    }
}
