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
