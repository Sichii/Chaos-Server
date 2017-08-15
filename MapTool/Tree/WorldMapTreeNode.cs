using System.Windows.Forms;

namespace MapTool
{
    internal class WorldMapTreeNode : TreeNode
    {
        internal WorldMap WorldMap { get; }

        internal WorldMapTreeNode(WorldMap worldMap, string name)
            : base(name)
        {
            WorldMap = worldMap;
            Name = worldMap.GetCrc32().ToString();
        }
    }
}
