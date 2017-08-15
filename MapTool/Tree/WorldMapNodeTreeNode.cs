using System.Windows.Forms;

namespace MapTool
{
    internal class WorldMapNodeTreeNode : TreeNode
    {
        internal WorldMapNode WorldMapNode { get; }

        internal WorldMapNodeTreeNode(WorldMapNode worldMapNode, string name)
            : base(name)
        {
            WorldMapNode = worldMapNode;
            Name = WorldMapNode.CRC.ToString();
        }
    }
}
