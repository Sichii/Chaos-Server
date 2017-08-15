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
