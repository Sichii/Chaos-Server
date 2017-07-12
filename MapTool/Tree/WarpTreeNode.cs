using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapTool
{
    internal class WarpTreeNode : TreeNode
    {
        internal Warp Warp { get; }

        internal WarpTreeNode(Warp warp, string name)
            : base(name)
        {
            Warp = warp;
            Name = warp.Point.ToString();
        }
    }
}
